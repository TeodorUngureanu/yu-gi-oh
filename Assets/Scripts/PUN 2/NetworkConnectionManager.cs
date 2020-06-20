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

    public InputField createRoomTF;
    public InputField joinRoomTF;

    public void OnClick_ConnectBtn()
    {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.NickName = "PlayerName";
        PhotonNetwork.GameVersion = "v1";

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

        PhotonNetwork.LoadLevel("scene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Joined Failed " + returnCode + ", Message " + message);
    }
}
