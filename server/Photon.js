const express = require("express");
const bodyParser = require("body-parser");
const cors = require("cors");
const WebSocket = require("ws");
const { v4: uuidv4 } = require('uuid');
const app = express();
app.use(
  cors({
    origin: "*",
    credentials: true,
  })
);
const PORT = process.env.PORT || 8000;
const wss = new WebSocket.Server({ port: 5000 });
let lobbyMap = new Map(); // Initialize a map
let clientUserNameMap = new Map();
let clientUserIdMap = new Map();

app.use(express.json());
app.use(express.urlencoded({ extended: true }));
app.use(bodyParser.text()); // Parse request body as text

// Websocket code start
function handleMessage(message, ws) {
  message = message.toString();
  console.log(message);
  if (message.startsWith("createLobby")) {
    console.log("creating lobby ", message);
    const receivedInfo = message.split(",");
    const receivedLobbyCode = receivedInfo[1]; // Extract the lobby code from the message
    const receivedUsername = receivedInfo[2]; // Extract the username from the message
    if (
      receivedLobbyCode &&
      receivedUsername &&
      !lobbyMap.has(receivedLobbyCode)
    ) {
      // Lobby code doesn't exist, create a new array with the user and set it in the map
      lobbyMap.set(receivedLobbyCode, [receivedUsername]);

      console.log(
        "New lobby created with code:",
        receivedLobbyCode,
        "by user:",
        receivedUsername
      );
      // Optionally, you can send a confirmation message back to the client
      ws.send("Lobby created with code: " + receivedLobbyCode);
    } else {
      // Lobby with the provided code already exists
      console.log("Lobby with code", receivedLobbyCode, "already exists.");
      // Optionally, you can send an error message back to the client
      ws.send("Error: Lobby with this code already exists.");
    }
  } else if (message.startsWith("joinLobby")) {
    console.log(message);
    const receivedInfo = message.split(",");
    const receivedLobbyCode = receivedInfo[1]; // Extract the lobby code from the message
    const receivedUsername = receivedInfo[2]; // Extract the username from the message
    if (lobbyMap.has(receivedLobbyCode)) {
      const users = lobbyMap.get(receivedLobbyCode);
      users.push(receivedUsername);
      lobbyMap.set(receivedLobbyCode, users);
      // Lobby with the provided code exists
      console.log("Player joined lobby with code:", receivedLobbyCode);
      // Optionally, you can send a success message back to the client
      ws.send("LobbyJoined:" + receivedLobbyCode); // Send the LobbyJoined message
    
    }  
    
    else {
      // Lobby with the provided code doesn't exist
      console.log(
        "Error: Lobby with code",
        receivedLobbyCode,
        "does not exist."
      );
      // Optionally, you can send an error message back to the client
      ws.send("Error: Lobby with this code does not exist.");
    }
  } else if (message.startsWith("sendUsername")) {
    // Handle the sendUsername message
    const receivedInfo = message.split(",");
    const receivedUsername = receivedInfo[1]; // Extract the username from the message

    // Optionally, you can perform additional logic here with the received username
    console.log("Received username: Line no72", receivedUsername);
   // const clientId = uuidv4();
    clientUserNameMap.set(receivedUsername,ws.clientId);
    console.log("Mapping progress: 75",receivedUsername,ws.clientId);
    // Respond to the client that the username has been received
    ws.send("mappingDone " + ws.clientId);
}else if (message.startsWith("sendUserID")) {
  // Handle the sendUsername message
  const receivedInfo = message.split(",");
  const receivedUserId = receivedInfo[1]; // Extract the username from the message

  // Optionally, you can perform additional logic here with the received username
  console.log("Received username: Line no72", receivedUserId);
 // const clientId = uuidv4();
  clientUserIdMap.set(receivedUserId,ws.clientId);
  console.log("101",receivedUserId,ws.clientId);
  // Respond to the client that the username has been received
  ws.send("103 " + ws.clientId);
}else if (message.startsWith("reactToUnity")) {
  console.log("105",message);
}
}
wss.on("connection", function connection(ws) {

   // Generate a clientId for the user
   const clientId = uuidv4();

   // Associate the clientId with the WebSocket connection
  ws.clientId = clientId;

  console.log(`User connected with clientId: ${clientId}`);
  ws.on("message", function incoming(message) {
    console.log("received: %s", message);
    handleMessage(message, ws); // Pass the WebSocket connection to the handleMessage function
  });

  ws.send("connected");
});

// Websocket code end

app.get("/", (req, res) => {
  res.send("serverisUp");
});

app.post("/join-lobby-from-browser", (req, res) => {
  const { username, lobbyCodeFromBrowser } = req.body;
  console.log(`Joining user: ${username} to lobby ${lobbyCodeFromBrowser}`);

  const tempClientId = clientUserIdMap.get(username)
  wss.clients.forEach((client)=>{
    // console.log(client.clientId);
    if(client.clientId===tempClientId){
      console.log("client here joining");
      client.send("joinLobbyFromBrowser");
    }
  })
  return res.send("Joining successful");
});

app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
