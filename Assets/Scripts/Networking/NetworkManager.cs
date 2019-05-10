using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour {

    private static NetworkManager instance;

    public Button host;
    public Button join;
    public InputField ipAddress;

    Server server;
    Client client;

    public static NetworkManager Get()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        host.onClick.AddListener(ClickHost);
        join.onClick.AddListener(StartClient);
    }

    void ClickHost()
    {
        host.GetComponent<Image>().color = Color.black;
        host.GetComponentInChildren<Text>().color = Color.white;

        join.gameObject.SetActive(false);
        ipAddress.gameObject.SetActive(false);
        StartServer();
    }

    void StartServer()
    {
        GameManager.Get().SetFirst(true);
        server = new Server();
    }

    void StartClient()
    {
        GameManager.Get().SetFirst(false);
        client = new Client(ipAddress.text);
    }

    public void HideMultiplayerMenu() {
        host.gameObject.SetActive(false);
        join.gameObject.SetActive(false);
        ipAddress.gameObject.SetActive(false);

        GameManager.Get().InitDuel();
    }
}
