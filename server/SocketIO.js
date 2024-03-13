const express = require("express");
const http = require("http");
const socketIO = require("socket.io");
const cors = require("cors");

const app = express();

// Allow requests from all origins
app.use(cors());

const server = http.createServer(app);
const io = socketIO(server);

io.on("connection", (socket) => {
  console.log("A user connected");

  // Handle messages from Unity to React
  socket.on("unityToReact", (data) => {
    console.log("Message from Unity:", data);
    // Process the data as needed and emit to React if necessary
    io.emit("unityToReact", data);
  });

  // Handle messages from React to Unity
  socket.on("reactToUnity", (data) => {
    console.log("Message from React:", data);
    // Process the data as needed and emit to Unity if necessary
    io.emit("reactToUnity", data);
  });

  // Handle disconnect
  socket.on("disconnect", () => {
    console.log("A user disconnected");
  });
});

const port = 5000;
server.listen(port, () => {
  console.log(`Server listening on http://localhost:${port}`);
});
