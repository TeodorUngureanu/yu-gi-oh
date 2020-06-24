using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetworkConnectionManager : MonoBehaviourPunCallbacks
{
    public GameObject connectedScreen;
    public GameObject disconnectedScreen;
    public GameObject waitingLobby;

    public InputField createRoomTF;
    public InputField joinRoomTF;

    private void Awake()
    {
        OnClick_ConnectBtn();
    }

    public void OnClick_ConnectBtn()
    {
        string playerName = PlayerPrefs.GetString("PlayerName");

        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.NickName = "" + playerName;
        PhotonNetwork.GameVersion = "v1";
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected to Master!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        disconnectedScreen.SetActive(true);
        Debug.Log(cause);
    }

    public override void OnJoinedLobby()
    {
        if (disconnectedScreen.activeSelf)
        {
            disconnectedScreen.SetActive(false);
        }

        connectedScreen.SetActive(true);
    }

    public void OnClick_JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinRoomTF.text, null);
    }

    public void OnClick_CreateRoom()
    {
        PhotonNetwork.CreateRoom(createRoomTF.text, new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined Successfully, Master: " 
            + PhotonNetwork.IsMasterClient 
            + " | Players in Room: " 
            + PhotonNetwork.CurrentRoom.Name 
            + " - " 
            + PhotonNetwork.CurrentRoom.PlayerCount);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            connectedScreen.SetActive(false);
            waitingLobby.SetActive(true);
        }
        else
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("ChatMessage", RpcTarget.All, "Start_Game");
        }
    }
    void OnPhotonPlayerConnected()
    {
        Debug.Log("OnPhotonPlayerConnected " + PhotonNetwork.CountOfPlayers);
    }

    public void CancelConnectionAndReturnToMainMenu()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("mainMenu");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Joined Failed " + returnCode + ", Message " + message);
    }

    [PunRPC]
    void ChatMessage(string message)
    {
        if (message == "Start_Game")
        {
            PhotonNetwork.LoadLevel("scene");
        }
    }

}
