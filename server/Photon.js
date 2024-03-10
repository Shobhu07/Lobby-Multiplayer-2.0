const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors'); 
const app = express();
const PORT = process.env.PORT || 5000;


let activeLobbies = [];
let connectedClients = [];
app.use(cors())

app.use(bodyParser.text()); // Parse request body as text

app.get('/', (req,res) => {res.send(" send the code")}) 
app.post('/create-lobby', (req, res) => {
    const lobbyCode = req.body; // Lobby code is already a string
    console.log("Received lobby code:", lobbyCode); // Log received lobby code
    if (!lobbyExists(lobbyCode)) {
        // Lobby doesn't exist, add it to the list of active lobbies
        activeLobbies.push({ lobbyCode, players: [] });
        console.log('New lobby created with code:', lobbyCode);
        res.status(200).send('Lobby created with code: ' + lobbyCode);
    } else {
        console.log('Lobby with code', lobbyCode, 'already exists.');
        res.status(400).send('Error: Lobby with this code already exists.');
    }
});

function lobbyExists(lobbyCode) {
    return activeLobbies.some(lobby => lobby.lobbyCode === lobbyCode);
}

app.post('/join-lobby', (req, res) => {
    const lobbyCode = req.body;
    console.log("received lobbyCode " + lobbyCode)
    if (lobbyExists(lobbyCode)) {
        console.log('Player joined lobby with code:', lobbyCode);
        res.status(200).send('LobbyFound:' + lobbyCode);
    } else {
        console.log('Error: Lobby with code', lobbyCode, 'does not exist.');
        res.status(400).send('Error: Lobby with this code does not exist.');
    }
});

function lobbyExists(lobbyCode) {
    return activeLobbies.some(lobby => lobby.lobbyCode === lobbyCode);
}

app.post('/connection-established', (req, res) => {
    const clientIP = req.socket.remoteAddress;
    if (!connectedClients.includes(clientIP)) {
        connectedClients.push(clientIP);
        console.log(`Client connected from IP: ${clientIP}`);
    }
    res.sendStatus(200);
});

app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});
