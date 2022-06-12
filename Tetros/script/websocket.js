const offline = false;
var player_username = "";

var leaderboardEntryUsername = [];
var leaderboardEntryScore = [];

console.log("websocket.js Loaded");

// Create WebSocket connection.
if(!offline){
	console.log("Opening WebSocket Connection");
	var socket = new WebSocket('ws://localhost:80');	
}

// Connection opened
if(!offline){
	/* Connection Opened */
	socket.addEventListener('open', function (event) {
	console.log('Websocket Connection Open');

	// Checks for stored session Key, if it exists, sends key
	if(localStorage.sessionKey != null){
		console.log('Attempting To Restore Session');
		// Session key exists, send to server to retrieve session
		socket.send(
			'\r\n'+
			'type : SESSIONGET\r\n' +
			'sessionKey : ' + localStorage.sessionKey
			);

	}else{
		console.log('No Session Key Stored');
		localStorage.sessionKey = "";
	}

	});
}

// Receive - Message Handling
// Message Format - [Type:Message;Message2]
if(!offline){
	socket.addEventListener('message', function (event) {
	var message = event.data+"";
	var messagesplit = message.split(':');

	// Type
	var messageType = messagesplit[0];

	// Data
	var messageData = messagesplit[1].split(";");

	// Message Handling
	switch(messageType){
		case "SESSIONGET":
			if(messageData[0] === "true"){
				console.log("Session Restored. Username : " + messageData[1])
				player_username = messageData[1];
				loadMenu();
				menu_init();
			}else{
				console.log("Session Not Found");
			}
			break;

		case "AUTHRESULT":
			if(messageData[0] === "true"){
				console.log("Auth Successful.");
				console.log("New Session Key : " + messageData[2]);
				player_username = messageData[1];
				localStorage.sessionKey = messageData[2];
				loadMenu();
				menu_init();
			}else{
				alert("Login Failed");
				console.log("Auth Failed");
			}
			break;

		case "LEADERBOARD":
			var leaderboardIndex = 0;
			console.log("LEADERBOARD");
			for (let i = 0; i < messageData.length; i++) {
				if(messageData[i] != ""){
					leaderboardEntryUsername[leaderboardIndex] = messageData[i];
					leaderboardEntryScore[leaderboardIndex] = messageData[i+1];
					i++;
					leaderboardIndex++;
				}
			}
			break;

		case "LOGOUT":
			console.log("Logged Out");
			alert("Logged Out");
			break;

		case "ERROR":
			switch(messageData[0]){
				case "UnknownCommand":
					console.log("Unknown Command");
					break;
				
				default :
					console.log("Unknown Error");
					break;
			}
			break;

		default:

			break;
	}
	
	});	
}

//Login
function login () {
	const input_username = document.getElementById("login_form_username_input");
	const input_password = document.getElementById("login_form_password_input");

	if(!offline){
		socket.send(
	 	'\r\n'+
	 	'type : ' + 'LOGIN' + '\r\n' +
	 	'username : ' + input_username.value + '\r\n' +
	 	'password : ' + input_password.value
		);
	}else{
		console.log(
	 	'\r\n'+
	 	'type : ' + 'LOGIN' + '\r\n' +
	 	'username : ' + input_username.value + '\r\n' +
	 	'password : ' + input_password.value
		);
	}
	
}