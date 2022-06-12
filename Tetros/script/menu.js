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
    socket.send("type : GETLEADERBOARD");
    setTimeout(populateLeaderboard,250);
}

function populateLeaderboard(){
    leaderboardElement = document.getElementById("leaderboard");

    for (let pli = 0; pli < leaderboardEntryUsername.length; pli++) {
        // Leaderboard Entry
        var leaderboard_entry = document.createElement("div");
        leaderboard_entry.setAttribute("id","leaderboard_entry");

        // Username
        var leaderboard_entry_username = document.createElement("text");
        leaderboard_entry_username.setAttribute("id","leaderboard_entry_username");
        leaderboard_entry_username.innerText = leaderboardEntryUsername[pli];

        // Score
        var leaderboard_entry_score = document.createElement("text");
        leaderboard_entry_score.setAttribute("id","leaderboard_entry_score");
        leaderboard_entry_score.innerText = leaderboardEntryScore[pli];

        // Add Children
        leaderboard_entry.appendChild(leaderboard_entry_username);
        leaderboard_entry.appendChild(leaderboard_entry_score);

        // Add Entry
        leaderboardElement.appendChild(leaderboard_entry);
    }
}