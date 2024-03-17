using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class GameController : MonoBehaviourPunCallbacks
{
    private static GameController s_Instance;
    public LobbyManager lobbymanager;
    private string lobbyCodeToJoin;
    public static string userID;

    [DllImport("__Internal")]
    private static extern void SendUserIdToReact(string userId);

    [DllImport("__Internal")]
    public static extern void SendLobbyCodeToReact(string LobbyCode);

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

 /*   public void SomeMethod()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    GameOver ("Player1", 100);
#endif
    } */

    // Start is called before the first frame update
    void Start()
    {
        // Attempt to load existing user ID
        userID = PlayerPrefs.GetString("UserID");

        // If no user ID found, generate a new one
        if (string.IsNullOrEmpty(userID))
        {
            userID = GenerateUserID();
            PlayerPrefs.SetString("UserID", userID);
            PlayerPrefs.Save();
      
        }
        SendUserIdToReact(userID);
    }

    private string GenerateUserID()
    {
        // Generate a new GUID (Globally Unique Identifier)
        return Guid.NewGuid().ToString();
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

   

