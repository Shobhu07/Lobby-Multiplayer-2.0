const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 5000 });
let activeLobbies = []; // Initialize an array to store active lobbies

wss.on('connection', function connection(ws) {
    ws.on('message', function incoming(message) {
        console.log('received: %s', message);
        handleMessage(message, ws); // Pass the WebSocket connection to the handleMessage function
    });

    ws.send('connected');
});

function handleMessage(message, ws) {
    message = message.toString(); // Convert message to string if it's not already
    if (message.startsWith('LobbyCode:')) {
        const lobbyCode = message.substring(10); // Extract the lobby code from the message
        if (!lobbyExists(lobbyCode)) {
            // If the lobby doesn't exist, add it to the list of active lobbies
            activeLobbies.push({ lobbyCode, players: [] });
            console.log('New lobby created with code:', lobbyCode);
            // Optionally, you can send a confirmation message back to the client
            ws.send('Lobby created with code: ' + lobbyCode);
        } else {
            // Lobby with the provided code already exists
            console.log('Lobby with code', lobbyCode, 'already exists.');
            // Optionally, you can send an error message back to the client
            ws.send('Error: Lobby with this code already exists.');
        }
    } else if (message.startsWith('JoinLobbyCode:')) {
        const lobbyCode = message.substring(14); // Extract the lobby code from the message
        if (lobbyExists(lobbyCode)) {
            // Lobby with the provided code exists
            console.log('Player joined lobby with code:', lobbyCode);
            // Optionally, you can send a success message back to the client
            ws.send('LobbyJoined:' + lobbyCode); // Send the LobbyJoined message
        } else {
            // Lobby with the provided code doesn't exist
            console.log('Error: Lobby with code', lobbyCode, 'does not exist.');
            // Optionally, you can send an error message back to the client
            ws.send('Error: Lobby with this code does not exist.');
        }
    } else {
        // Handle other types of messages here if needed
    }
}

function lobbyExists(lobbyCode) {
    // Check if the provided lobby code exists in the list of active lobbies
    return activeLobbies.some(lobby => lobby.lobbyCode === lobbyCode);
}
