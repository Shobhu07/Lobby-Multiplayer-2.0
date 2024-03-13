const WebSocket = require('ws');
const express = require('express');
const cors = require('cors');

const app = express();
app.use(cors());

const wss = new WebSocket.Server({ noServer: true });

const server = app.listen(5000, () => {
    console.log('WebSocket server listening on port 5000');
});

server.on('upgrade', (request, socket, head) => {
    wss.handleUpgrade(request, socket, head, (ws) => {
        wss.emit('connection', ws, request);
    });
});


let lobbyMap = new Map(); // Initialize an array to store active lobbies


wss.on('connection', function connection(ws) {
    ws.on('message', function incoming(message) {
        console.log('received: %s', message);
        handleMessage(message, ws); // Pass the WebSocket connection to the handleMessage function
    });

    ws.send('connected');
});

function handleMessage(message, ws) {
    message = message.toString();
  console.log(message);
    if (message.startsWith('createLobby')) {
        console.log("creating lobby ",message);
        const receivedInfo = message.split(",");
        const receivedLobbyCode = receivedInfo[1]; // Extract the lobby code from the message
        const receivedUsername = receivedInfo[2]; // Extract the username from the message
        if (receivedLobbyCode && receivedUsername && !lobbyMap.has(receivedLobbyCode) ) {
                // Lobby code doesn't exist, create a new array with the user and set it in the map
                lobbyMap.set(receivedLobbyCode, [receivedUsername]);

                console.log('New lobby created with code:', receivedLobbyCode, 'by user:', receivedUsername);
                // Optionally, you can send a confirmation message back to the client
                ws.send('Lobby created with code: ' + receivedLobbyCode);
              

        } else {
            // Lobby with the provided code already exists
            console.log('Lobby with code', receivedLobbyCode, 'already exists.');
            // Optionally, you can send an error message back to the client
            ws.send('Error: Lobby with this code already exists.');
        }
    } else if (message.startsWith('joinLobby')) {
        console.log(message);
        const receivedInfo = message.split(",");
        const receivedLobbyCode = receivedInfo[1]; // Extract the lobby code from the message
        const receivedUsername = receivedInfo[2]; // Extract the username from the message
        if (lobbyMap.has(receivedLobbyCode)) {
            const users = lobbyMap.get(receivedLobbyCode);
            users.push(receivedUsername);
            lobbyMap.set(receivedLobbyCode, users);
            // Lobby with the provided code exists
            console.log('Player joined lobby with code:', receivedLobbyCode);
            // Optionally, you can send a success message back to the client
            ws.send('LobbyJoined:' + receivedLobbyCode); // Send the LobbyJoined message
        } else {
            // Lobby with the provided code doesn't exist
            console.log('Error: Lobby with code', receivedLobbyCode, 'does not exist.');
            // Optionally, you can send an error message back to the client
            ws.send('Error: Lobby with this code does not exist.');
        }  
    }  
}

