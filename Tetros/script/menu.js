console.log("menu.js loaded");

function menu_init(){
    console.log("Initializing Menu");
    setTimeout('usernameDisplay()',100);
}

function usernameDisplay(){
    document.getElementById('username_name').textContent = player_username;
}

function play(){
    loadGame();
    setTimeout('init()',250)
}

function logout(){
    socket.send("type : LOGOUT");
    loadLogin();
}

function leaderboard(){
    loadLeaderboard();
    //socket.send("type : GETLEADERBOARD");
}