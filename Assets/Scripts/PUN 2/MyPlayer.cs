using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyPlayer : MonoBehaviourPun, IPunObservable
{
    private float moveSpeed = 10;
    private float jumpForce = 800;
    // private Vector3 smoothMove;

    void Start()
    {
        Debug.Log("IsMine " + photonView.IsMine);
    }

    private void Update()
    {
        // Local Player
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
        else
        {
            SmoothMovement();
        }
    }

    private void ProcessInputs()
    {
        var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0);

        transform.position += move * moveSpeed * Time.deltaTime;
    }

    private void SmoothMovement()
    {
        // transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // stream.SendNext(transform.position);
            stream.SendNext("Message");
        }
        else if (stream.IsReading)
        {
            // smoothMove = (Vector3)stream.ReceiveNext();
            string message = (string)stream.ReceiveNext();
        }
    }

    public static void RefreshInstance(ref MyPlayer player, MyPlayer Prefab)
    {
        var position = Vector3.zero;
        var rotation = Quaternion.identity;

        if (player != null)
        {
            position = player.transform.position;
            rotation = player.transform.rotation;
            PhotonNetwork.Destroy(player.gameObject);
        }

        player = PhotonNetwork.Instantiate(Prefab.gameObject.name, position, rotation).GetComponent<MyPlayer>();
    }

    public void SendRPCMessage(string serializedMessage)
    {
        photonView.RPC("SendSerializedMessage", RpcTarget.Others, serializedMessage);
    }

    [PunRPC]
    void SendSerializedMessage(string serializedMessage, PhotonMessageInfo info)
    {
        GameManager.Get().ReceiveInformation(serializedMessage);
    }
}
