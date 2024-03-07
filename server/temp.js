const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 5000 });

let nextUserId = 1;

let lobbies = [];

wss.on('connection', (ws) => {
    const userId = nextUserId++;
    ws.send(JSON.stringify({ type: 'user_id', userId }));
    console.log(`User ${userId} connected`);

    ws.on('message', (message) => {
      console.log(typeof(message));
        if (message.toString() === 'create_lobby') {
           console.log(`Helllo ${message}`);
            const newLobby = {
                id: Math.ceil(Math.random()*1000),
                players: [userId]
            };
            lobbies.push(newLobby);
            const lobbyCode = newLobby.id;
            console.log(`Lobby created with code: ${lobbyCode}`);
            ws.send(JSON.stringify({ type: 'lobby_code', lobbyCode: lobbyCode }));
        }else if (message.startsWith('join_lobby:')) {
            const lobbyCode = message.split(':')[1];
            const lobby = lobbies.find(lobby => lobby.id === parseInt(lobbyCode));
            if (lobby) {
                lobby.players.push(userId);
                console.log(`User ${userId} joined lobby ${lobbyCode}`);
                ws.send(JSON.stringify({ type: 'lobby_joined', lobbyCode }));
            } else {
                console.log(`Lobby ${lobbyCode} not found`);
                ws.send(JSON.stringify({ type: 'lobby_not_found', lobbyCode }));
            }
        }
    });
});