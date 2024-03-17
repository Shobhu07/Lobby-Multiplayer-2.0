using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    public TextMeshProUGUI buttonText;
    public WebsocketConnection websocketConnection; // Corrected variable type

    private void Start()
    {
        // Get a reference to the WebsocketConnection script
        websocketConnection = FindObjectOfType<WebsocketConnection>();
    }
    public void OnCLickConnect()
    {
        if (usernameInput != null && !string.IsNullOrEmpty(usernameInput.text))
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "Connecting....";

            string username = PhotonNetwork.NickName;
            // Send the username to the backend server
            websocketConnection.SendUsernameToServer(username);

            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
