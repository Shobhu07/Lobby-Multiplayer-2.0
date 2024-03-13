  // Unity will call this function to establish the WebSocket connection
  function ConnectToWebSocket() {
    socket = new WebSocket("ws://localhost:5000");
    socket.onopen = OnWebSocketOpen;
    socket.onmessage = OnWebSocketMessage;
    socket.onclose = OnWebSocketClose;
    socket.onerror = OnWebSocketError;

     // Define SendWebSocketMessage function here
     function SendWebSocketMessage(message) {
        if (socket.readyState === WebSocket.OPEN) {
            socket.send(message);
            console.log("Sent message to server:", message);
        } else {
            console.error("WebSocket connection is not open.");
        }
        
}
}
// Callback functions for WebSocket events
function OnWebSocketOpen(event) {
    console.log("WebSocket connection established.");
}

function OnWebSocketMessage(event) {
    var message = event.data;
    console.log("Received message: " + message);
    // Handle the received message as needed
    // Call a Unity function if needed
    SendMessageToUnity("OnWebSocketMessage", message);
}

function OnWebSocketClose(event) {
    console.log("WebSocket connection closed.");
}

function OnWebSocketError(event) {
    console.error("WebSocket error:", event);
}

// Function to send a message to the server
function SendLobbyCode(lobbyCode, username) {
    if (socket.readyState === WebSocket.OPEN) {
        var message = `createLobby,${lobbyCode},${username}`;
        socket.send(message);
        console.log("Sent message to server:", message);
    } else {
        console.error("WebSocket connection is not open.");
    }
}

function SendJoinLobbyCode(lobbyCode, username) {
    if (socket.readyState === WebSocket.OPEN) {
        var message = `joinLobby,${lobbyCode},${username}`;
        socket.send(message);
        console.log("Sent message to server:", message);
    } else {
        console.error("WebSocket connection is not open.");
    }
}

function onJoinLobbySuccess(lobbyCode) {
        // Handle the lobby join success
        console.log("Joined lobby with code: " + lobbyCode);
        // Perform additional actions if needed
    }

// Function to send a message to Unity
function SendMessageToUnity(functionName, messageData) {
    var unityInstance = UnityInstance.getInstance();
    var sendMessage = messageData
        ? unityInstance.SendMessage
        : unityInstance.SendMessageUnityInstance;
    sendMessage("WebsocketConnection", functionName, messageData);
}