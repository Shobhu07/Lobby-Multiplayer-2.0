const WebSocket = require('ws');
const wss = new WebSocket.Server({ port: 5000 });

let nextUserId = 1;
let lobbies = [];

wss.on('connection', (ws) => {
    const userId = nextUserId++;
    ws.send(JSON.stringify({ type: 'user_id', userId }));
    console.log(`User ${userId} connected`);

    ws.on('message', (message) => {
        console.log(`Received: ${message}`);
        
        // Convert message to string
        message = message.toString();

        // Check if the message is 'create_lobby'
if (message === 'create_lobby') {
  const newLobby = {
      id: Math.ceil(Math.random() * 1000),
      players: [userId]
  };
  lobbies.push(newLobby);
  const lobbyCode = newLobby.id;
  console.log(`Lobby created with code: ${lobbyCode}`);
  // Construct lobby scene name
  const lobbySceneName = `LobbyScene_${lobbyCode}`;
  // Send lobby code and scene name back to the client
  ws.send(JSON.stringify({ type: 'lobby_code', lobbyCode: lobbyCode }));
  // Send lobby scene name to the joining user
  console.log(`Sending lobby scene name to User ${userId}: ${lobbySceneName}`);
  ws.send(JSON.stringify({ type: 'lobby_scene', sceneName: lobbySceneName }));
} 
          // Check if the message starts with 'join_lobby:'
          else if (message.startsWith('join_lobby:')) {
            const lobbyCode = message.split(':')[1];
            const lobby = lobbies.find(lobby => lobby.id === parseInt(lobbyCode));
            if (lobby) {
                lobby.players.push(userId);
                console.log(`User ${userId} joined lobby ${lobbyCode}`);
                // Send the updated count of players in the lobby back to the client
                const playerCount = lobby.players.length;
                ws.send(JSON.stringify({ type: 'lobby_joined', playerCount }));

                // Send lobby scene name to the joining user
                const lobbySceneName = `LobbyScene_${lobbyCode}`;
                ws.send(JSON.stringify({ type: 'lobby_scene', sceneName: lobbySceneName }));
                // Send success response back to the client
                ws.send(JSON.stringify({ type: 'lobby_join_response', response: 'success' }));
            } else {
                console.log(`Lobby ${lobbyCode} not found`);
                // Send error response back to the client
                ws.send(JSON.stringify({ type: 'lobby_join_response', response: 'error', message: 'Lobby not found' }));
            }
      }
       
        
      }
  );
});