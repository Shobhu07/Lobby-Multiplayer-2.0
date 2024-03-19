using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveLobby : MonoBehaviour
{
    public LobbyManager manager;

   public void OnLeaveLobby()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(1);

        

        string LobbyCode =  PlayerPrefs.GetString("lobbyCode");
       string username = PhotonNetwork.NickName;     
       GameController.SendDeleteInformation(LobbyCode,username);
    }
}
