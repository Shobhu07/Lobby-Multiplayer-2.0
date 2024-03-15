using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class GameController : MonoBehaviourPunCallbacks
{
    private static GameController s_Instance;
    private WebsocketConnection websocketObject;
    public LobbyManager lobbymanager;
    private string lobbyCodeToJoin;

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

      
    }

    // Start is called before the first frame update
    void Start()
    {  // Obtain reference to the WebsocketConnection script
        websocketObject = GetComponent<WebsocketConnection>();
       
        

        // Ensure websocketObject is not null
        if (websocketObject != null)
        {
            // Get the userID from WebsocketConnection
            string userID = websocketObject.GetUserID();
            Debug.Log("UserID obtained in GameController: " + userID);
            SendMessageToReact(userID);
        }
        else
        {
            Debug.LogError("WebsocketConnection component not found.");
        }
    }

    private void SendMessageToReact(string message)
    {
        // Ensure that the method is not called recursively
        if (gameObject != null)
        {
            // Send a message to the React component
            gameObject.SendMessage("ReceiveMessageFromUnity", message);
        }
        else
        {
            Debug.LogWarning("GameObject is null. Unable to send message to React.");
        }
    }

    public void ReceiveMessageFromReact(string message)
    {
        Debug.Log($"Received message from React: {message}");

    }

    public void LobbyCodeFromBrowser(string lobbycode)
    {
        Debug.Log($"Lobby code from browser {lobbycode}");
        if (!string.IsNullOrEmpty(lobbycode))
        {
            ConnectToPhotonAndJoinLobby(lobbycode);
        }
        else
        {
            Debug.LogError("Invalid lobby code received from browser.");
        }
    }

    private async void ConnectToPhotonAndJoinLobby(string lobbycode)
    {
        // Connect to Photon
        await ConnectToPhoton();
        // Store the lobby code to join later
        lobbyCodeToJoin = lobbycode;

    }

    private async Task ConnectToPhoton()
    {
        // Connect to Photon
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        // Wait until connected to Photon
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            await Task.Yield();
        }

        Debug.Log("Connected to Photon!");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster callback received.");

        // Join the default lobby
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby callback received.");

        // If there's a lobby code to join, call OnJoinSuccess
        if (!string.IsNullOrEmpty(lobbyCodeToJoin))
        {
            lobbymanager.OnJoinSuccess(lobbyCodeToJoin);
            lobbyCodeToJoin = null; // Reset lobby code after joining
        }
    }

}

   

