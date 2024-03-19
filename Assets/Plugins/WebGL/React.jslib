mergeInto(LibraryManager.library, {

  SendUserIdToReact: function (userId){
	 window.dispatchReactUnityEvent("SendUserIdToReact", UTF8ToString(userId))
},
 SendLobbyCodeToReact: function (LobbyCode,username){
	 window.dispatchReactUnityEvent("SendLobbyCodeToReact", UTF8ToString(LobbyCode),UTF8ToString(username))
},
SendDeleteInformation: function (LobbyCode,username){
 window.dispatchReactUnityEvent("SendDeleteInformation", UTF8ToString(LobbyCode),UTF8ToString(username))
},

SendJoinLobbyRequest: function (LobbyCode,username){
 window.dispatchReactUnityEvent("SendJoinLobbyRequest", UTF8ToString(LobbyCode),UTF8ToString(username))
},


});