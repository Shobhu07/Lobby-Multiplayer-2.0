using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Threading;


public class WebSocketConnection : MonoBehaviour
{
    private ClientWebSocket webSocket;
    private static WebSocketConnection instance;
    private Action<string> joinLobbySuccessCallback;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ConnectToWebSocket();
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private async void ConnectToWebSocket()
    {
        try
        {
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri("ws://localhost:5000"), CancellationToken.None);
            OnWebSocketOpen();
            ReceiveLoop();
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

    private async void ReceiveLoop()
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
                        joinLobbySuccessCallback?.Invoke(lobbyCode);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in ReceiveLoop: {ex.Message}");
        }
    }

    public async void SendLobbyCode(string lobbyCode)
    {
        try
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("LobbyCode:" + lobbyCode);
                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                Debug.LogWarning("WebSocket connection is not open or WebSocket instance is null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending lobby code: {ex.Message}");
        }
    }

    public async void SendJoinLobbyCode(string lobbyCode, Action<string> onJoinSuccess)
    {
        try
        {
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                joinLobbySuccessCallback = onJoinSuccess;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("JoinLobbyCode:" + lobbyCode);
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

    private void OnApplicationQuit()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
    }
}