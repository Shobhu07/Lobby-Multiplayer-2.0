using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roominputfield;
    public TMP_InputField joininputfield;
    public WebsocketConnection websocketConnection;


    private void Start()
    {
        PhotonNetwork.JoinLobby();
        websocketConnection = FindObjectOfType<WebsocketConnection>();
    }

    public void OnClickCreate()
    {
        if (roominputfield != null)
        {
            string lobbyCode = roominputfield.text;
            string username = PhotonNetwork.NickName; 



            PhotonNetwork.CreateRoom(roominputfield.text, new RoomOptions() { MaxPlayers = 3 });

            // Send the lobby code to the backend server
            websocketConnection.SendLobbyCode(lobbyCode, username);
        }
    }

    public void JoinRoom()
    {
        if (joininputfield != null)
        {
            string lobbyCode = joininputfield.text;
            string username = PhotonNetwork.NickName;
         
            websocketConnection.SendJoinLobbyCode(lobbyCode,username, OnJoinSuccess);
        }
    }

    public void OnJoinSuccess(string lobbyCode)
    {
        
        PhotonNetwork.JoinRoom(lobbyCode);
    }   
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    
}
