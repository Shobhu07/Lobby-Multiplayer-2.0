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
    private GameController gameController;  
  


    private void Start()
    {
        PhotonNetwork.JoinLobby();


        gameController = FindObjectOfType<GameController>();
    }

    public void OnClickCreate()
    {
        if (roominputfield != null && !string.IsNullOrEmpty(roominputfield.text))
        {
            string lobbyCode = roominputfield.text;

            PlayerPrefs.SetString("lobbyCode", lobbyCode);
         
            string username = PhotonNetwork.NickName;

            PhotonNetwork.CreateRoom(roominputfield.text, new RoomOptions() { MaxPlayers = 3 });

            // Send lobby code to React using GameController's method
            GameController.SendLobbyCodeToReact(lobbyCode, username);

        }
    }

    public void JoinRoom()
    {
        if (joininputfield != null && !string.IsNullOrEmpty(joininputfield.text))
        {
            string lobbyCode = joininputfield.text;
            string username = PhotonNetwork.NickName;

            // websocketConnection.SendJoinLobbyCode(lobbyCode,username, OnJoinSuccess);

            GameController.SendJoinLobbyRequest(lobbyCode, username);

            OnJoinSuccess(lobbyCode);

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
