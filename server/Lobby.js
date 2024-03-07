const WebSocket = require('ws');

const wss = new WebSocket.Server({ port: 8080 });
let activeLobbies = []; // Initialize an array to store active lobbies

wss.on('connection', function connection(ws) {
  ws.on('message', function incoming(message) {
    console.log('received: %s', message);
    handleMessage(message, ws); // Pass the WebSocket connection to the handleMessage function
  });

  ws.send('connected');
});

function handleMessage(message, ws) {
  // Convert the message to string to ensure compatibility
  message = message.toString();

 // Check if the message contains lobby details
 if (message.startsWith("LobbyDetails:")) {
  // Extract lobby details from the message
  const lobbyDetails = message.split(":")[1].trim().split(",");

  // Validate lobby details
  if (lobbyDetails.length === 3) { // Check if all required details are provided
    const lobbyCode = lobbyDetails[0].trim(); // Extract lobby code from client message
    const maxPlayers = parseInt(lobbyDetails[1].trim());
    const lobbyName = lobbyDetails[2].trim();

    // Check if the lobby code is unique
    const isUniqueCode = !activeLobbies.some(lobby => lobby.lobbyCode === lobbyCode);

    if (isUniqueCode) {
      // Create a lobby object
      const lobby = {
        lobbyCode: lobbyCode,
        name: lobbyName,
        maxPlayers: maxPlayers,
        host: ws // Store the WebSocket connection of the host
      };

      // Add the lobby to the list of active lobbies
      activeLobbies.push(lobby);
      console.log("Lobby added to list:", lobby);

      // Send success message to the client
      ws.send('LobbyCreated:' + lobby.lobbyCode); // You can customize this message as needed
    } else {
      // Send error message if the lobby code is not unique
      ws.send('LobbyCodeNotUnique');
    }
  } else {
    // Send error message if the message format is incorrect
    ws.send('InvalidMessageFormat');
  }
} else if (message.startsWith("JoinLobby:")) {
  // Extract the lobby code from the message
  const lobbyCode = message.split(":")[1].trim();

  // Check if the lobby code exists in the list of active lobbies
  const isLobbyExists = lobbyExists(lobbyCode);

  if (isLobbyExists) {
      // Send success message to the client indicating successful lobby joining
      ws.send('LobbyJoined:' + lobbyCode); // You can customize this message as needed

      // You can also send additional details of the lobby to the client if required

      // Perform any additional actions required for joining the lobby
  } else {
      // Send error message to the client indicating invalid lobby code
      ws.send('InvalidLobbyCode'); // You can customize this message as needed
  }
}
}

function lobbyExists(lobbyCode) {
  // Check if the provided lobby code exists in the list of active lobbies
  return activeLobbies.some(lobby => lobby.lobbyCode === lobbyCode);
}
