using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SSEConnection : MonoBehaviour
{
    private static SSEConnection instance;
    public static SSEConnection Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SSEConnection>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("APIConnection");
                    instance = obj.AddComponent<SSEConnection>();
                }
            }
            return instance;
        }
    }

    private const string SERVER_URL = "http://localhost:5000"; // Replace with your server URL
    private bool isConnected = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            EstablishConnection();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EstablishConnection()
    {
        // Connection logic here
        isConnected = true;
        Debug.Log("Connection established!");

        // Send a message to the backend server
        StartCoroutine(SendConnectionEstablishedMessage());
    }

    private IEnumerator SendConnectionEstablishedMessage()
    {
        string url = SERVER_URL + "/connection-established";
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Connection established message sent to server.");
            }
            else
            {
                Debug.LogError("Failed to send connection established message: " + request.error);
            }
        }
    }

    public void SendRequest(string endpoint, string data, System.Action<string> callback)
    {
        StartCoroutine(SendRequestCoroutine(endpoint, data, callback));
    }

    private IEnumerator SendRequestCoroutine(string endpoint, string data, System.Action<string> callback)
    {
        string url = SERVER_URL + endpoint;
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, data))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                callback(responseText);
            }
            else
            {
                Debug.LogError("Request failed: " + request.error);
            }
        }
    }
}