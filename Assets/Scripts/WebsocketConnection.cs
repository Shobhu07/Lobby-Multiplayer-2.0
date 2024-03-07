using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class WebsocketConnection : MonoBehaviour
{
    private static WebsocketConnection s_Instance;
    private Action<string> joinLobbySuccessCallback;

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

    public void CreateLobby(string lobbyCode)
    {
        StartCoroutine(CreateLobbyRequest(lobbyCode));
    }



    IEnumerator CreateLobbyRequest(string lobbyCode)
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost:5000/create-lobby", ""))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(lobbyCode);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.uploadHandler.contentType = "text/plain";

            yield return www.SendWebRequest();
            Debug.Log(www);
            Debug.Log(UnityWebRequest.Result.Success);

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                Debug.Log(responseText);
            }
            else
            {
                Debug.Log("Not received anything");
            }
        }
    }
    public void SendJoinLobbyCode(string lobbyCode, Action<string> onJoinSuccess)
    {
        joinLobbySuccessCallback = onJoinSuccess;
        StartCoroutine(SendJoinLobbyCodeRequest(lobbyCode));
    }

    IEnumerator SendJoinLobbyCodeRequest(string lobbyCode)
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost:5000/join-lobby", ""))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(lobbyCode);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.uploadHandler.contentType = "text/plain";

            yield return www.SendWebRequest();
            Debug.Log(www);
            Debug.Log(www.result);

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                Debug.Log(responseText);
                if (responseText.StartsWith("LobbyFound:"))
                {
                    // Extract the lobby code from the response
                    string foundLobbyCode = responseText.Substring("LobbyFound:".Length);
                    // Call the callback function with the found lobby code
                    joinLobbySuccessCallback?.Invoke(foundLobbyCode);
                }
            }
            else
            {
                Debug.Log("Not received anything");
            }
        }
    }
}
