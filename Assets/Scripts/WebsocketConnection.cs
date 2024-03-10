using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

public class WebsocketConnection : MonoBehaviour
{
    private ClientWebSocket webSocket;
    private static WebsocketConnection s_Instance;
    private Action<string> joinLobbySuccessCallback;

    private async void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
            await ConnectToWebSocket();
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

    private async Task ConnectToWebSocket()
    {
        try
        {
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
            OnWebSocketOpen();
            await ReceiveLoop();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error connecting to WebSocket: {ex.Message}");
        }
    }

    private void OnWebSocketOpen()
    {
        Debug.Log("WebSocket connection established");
    }

    private async Task ReceiveLoop()
    {
        try
        {
            byte[] buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
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

                  
                }

            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in ReceiveLoop: {ex.Message}");
        }
    }

    public async void SendLobbyCode(string lobbyCode, string username)
    {
        try
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                // Combine lobby code and username into a single message
                string message = $"createLobby,{lobbyCode},{username}";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
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

    public async void SendJoinLobbyCode(string lobbyCode,string username, Action<string> onJoinSuccess)
    {
        try
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                // Set the join lobby success callback
                joinLobbySuccessCallback = onJoinSuccess;
                // Send the join lobby code to the server
                string message = $"joinLobby,{lobbyCode},{username}";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
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
