using UnityEngine;
using BestHTTP.WebSocket;
using System;
using System.Threading;
using System.Text;

public class WebsocketConnection : MonoBehaviour
{
    private WebSocket webSocket;
    private static WebsocketConnection s_Instance;
    private Action<string> joinLobbySuccessCallback;
    private string userID;

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
            ConnectToWebSocket();
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

    private void Start()
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

        Debug.Log("User ID: " + userID);

        // Send the user ID to the server
        SendUserIDToServer(userID);
    }

    private string GenerateUserID()
    {
        // Generate a new GUID (Globally Unique Identifier)
        return Guid.NewGuid().ToString();
    }

    private void ConnectToWebSocket()
    {
        try
        {
            webSocket = new WebSocket(new Uri("ws://localhost:5000"));

            // Subscribe to WebSocket events
            webSocket.OnOpen += OnWebSocketOpen;
            webSocket.OnMessage += OnWebSocketMessageReceived;
            webSocket.OnError += OnWebSocketError;
            webSocket.OnClosed += OnWebSocketClosed;

            // Start the WebSocket connection
            webSocket.Open();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error connecting to WebSocket: {ex.Message}");
        }
    }

    private void OnWebSocketOpen(WebSocket ws)
    {
        Debug.Log("WebSocket connection established");
    }

    private void OnWebSocketMessageReceived(WebSocket ws, string message)
    {
        Debug.Log($"Received message: {message}");

        if (message.StartsWith("LobbyJoined:"))
        {
            string lobbyCode = message.Split(':')[1].Trim();
            Debug.Log("Joined lobby with code: " + lobbyCode);
            // Invoke the callback with the lobby code
            joinLobbySuccessCallback?.Invoke(lobbyCode);
        }
        else if (message.StartsWith("joinLobbyFromBrowser"))
        {
            Debug.Log("Data Received for request from beowser");
        }
        else if (message.StartsWith("mappingDone"))
        {
           
            Debug.Log("Mapping Succesful"  );
        }
    }

    private void OnWebSocketError(WebSocket ws, string error)
    {
        Debug.LogError($"WebSocket error: {error}");
    }

    private void OnWebSocketClosed(WebSocket ws, UInt16 code, string message)
    {
        Debug.Log($"WebSocket connection closed. Code: {code}, Message: {message}");
    }

    private void SendUserIDToServer(string userID)
    {
        try
        {
            if (webSocket != null && webSocket.IsOpen)
            {
                // Send the user ID to the server
                string message = $"sendUserID,{userID}";
                webSocket.Send(message);
            }
            else
            {
                Debug.LogWarning("WebSocket connection is not open or WebSocket instance is null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending user ID: {ex.Message}");
        }
    }

    public void SendUsernameToServer(string username)
    {
        try
        {
            if (webSocket != null && webSocket.IsOpen)
            {
                // Send the username to the server
                string message = $"sendUsername,{username}";
                webSocket.Send(message);
            }
            else
            {
                Debug.LogWarning("WebSocket connection is not open or WebSocket instance is null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending username: {ex.Message}");
        }

    }

    public void SendLobbyCode(string lobbyCode, string username)
    {
        try
        {
            if (webSocket != null && webSocket.IsOpen)
            {
                // Combine lobby code and username into a single message
                string message = $"createLobby,{lobbyCode},{username}";
                webSocket.Send(message);
            }
            else
            {
                Debug.LogWarning("WebSocket connection is not open or WebSocket instance is null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending lobby code and username: {ex.Message}");
        }
    }

    public void SendJoinLobbyCode(string lobbyCode, string username, Action<string> onJoinSuccess)
    {
        try
        {
            if (webSocket != null && webSocket.IsOpen)
            {
                // Set the join lobby success callback      
                joinLobbySuccessCallback = onJoinSuccess;
                // Send the join lobby code to the server
                string message = $"joinLobby,{lobbyCode},{username}";
                webSocket.Send(message);
            }
            else
            {
                Debug.LogWarning("WebSocket connection is not open or WebSocket instance is null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending join lobby code: {ex.Message}");
        }
    }
}
