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


    private void Start()
    {

    }

    public void OnClickConnect()
    {
        if (usernameInput != null && !string.IsNullOrEmpty(usernameInput.text))
        {
            string username = usernameInput.text;

            // Save username to PlayerPrefs
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save();

            PhotonNetwork.NickName = username;
            buttonText.text = "Connecting....";

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
