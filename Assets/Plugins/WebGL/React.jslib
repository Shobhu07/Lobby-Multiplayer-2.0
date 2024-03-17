mergeInto(LibraryManager.library, {

  SendUserIdToReact: function (userId){
	 window.dispatchReactUnityEvent("SendUserIdToReact", UTF8ToString(userId))
},
 SendLobbyCodeToReact: function (LobbyCode){
	 window.dispatchReactUnityEvent("SendLobbyCodeToReact", UTF8ToString(LobbyCode))
},

});