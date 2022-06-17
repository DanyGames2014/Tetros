// -- VARIABLES --
// Canvas
let canvas;
let ctx;

let canvas_next;
let ctx_next;

// Game Settings
const field_columns = 10;
const field_rows = 20;

const next_columns = 4;
const next_rows = 4;

const block_size = 50;

var fallSpeed = 0;

// Game Loop
var gameLoop;
var gameTime = 0;
var score = 0;
var lost = false;
var linesCleared = 0;
var level = 1;
var fallprogress = 0;
var first = true;

// Active Piece
var active = false;
var activeX = 0;
var activeY = -4;
var activePiece = [[0,0,0,0],
				   [0,0,0,0],
				   [0,0,0,0],
				   [0,0,0,0]];
var emptyPiece =  [[0,0,0,0],
				   [0,0,0,0],
				   [0,0,0,0],
				   [0,0,0,0]];
var nextPiece =   [[0,0,0,0],
				   [0,0,0,0],
				   [0,0,0,0],
				   [0,0,0,0]];

// Game Field
var field = [];


// Text
var scoreText;
var levelText;

// Variables
var debug_log = true;

// Game Init
function init(){
	if(debug_log){console.log('Initializing Game')};

	if(debug_log){console.log('Creating Game Field')};
	createField();
	field = getEmptyField();
	redrawField();

	scoreText = document.getElementById('game_content_left_score');
	levelText = document.getElementById('game_content_left_level');

	if(debug_log){console.log('Game Ready')};

	lost = false;
	linesCleared = 0;
	gameTime = 0;
	score = 0;
	level = 1;
	fallprogress = 0;

	gameLoop = setInterval(update, 10);
	// var fuckit = setInterval(redrawField, 250);
}

// Main Game Loop
function update() {
	gameTime++;
	fallSpeed = level / 25;
	fallprogress += fallSpeed;
	if(!lost){
		level = Math.floor((linesCleared+10) / 10)

		scoreText.innerHTML = "Score : <br>" + score;
		levelText.innerHTML = "Level : <br>" + level;

		if(checkCollisionDown()){
			drawActive();
			putActive();
			activeY = -4;
			activeX = 4;
			active = false;
			lineClear();
		}

		if(active){
			if(fallprogress >= 1){
				removeActive();
				activeY++;
				drawActive();
				lineClear();
				fallprogress = 0;
			}
		}else{
			activeX = 4;
			activeY = -4;
			activePiece = nextPiece;
			if(first){
				nextPiece = randomPiece();
				activePiece = randomPiece();
				first = false;
			}else{
				nextPiece = randomPiece();
			}
				redrawNext();	
				drawActive();
				active = true;
		}	
	}
}

// Game Over Sequence
function gameOver () {
	clearInterval(gameLoop);
	console.log("Game Over");
	// console.table(field);
	socket.send(
		'\r\n'+
		'type : SCORESUBMIT\r\n' +
		'username : ' + player_username + '\r\n' +
		'score : ' + score
		);

	active = false;
	lost = true;
	linesCleared = 0;
	gameTime = 0;
	score = 0;
	level = 1;
	fallprogress = 0;
	activeX = 0;
	activeY = -4;

	field = getEmptyField(); redrawField();
	
	ctx.fillStyle = "#FF0000";
	ctx.font = "1px Arial";
	ctx.fillText("Game Over", 1, 9);

	setTimeout(gameOverAction, 1200);
}

function gameOverAction() {
	loadMenu();
	menu_init();
	
}

// Key Listener
document.onkeydown = function (e) {
    e = e || window.event;
    if(!lost){
    switch (e.keyCode) {

    	case 13:
    		removeActive();
    		activeY = -4;
			activeX = 4;
			active = false;
    		break;

    	// Left Arrow
    	case 37:
    		if(!checkCollisionLeft()){
    			removeActive();
    			activeX--;
    			drawActive();
    		}
    		break;

    	// Up Arrow
    	case 38:
    		/*removeActive();
    		activeY--;
    		drawActive();
    		*/
    		removeActive();
    		var tempPiece = rotateRight(activePiece);
    		activePiece = emptyPiece;
    		activePiece = tempPiece;
    		drawActive();
    		break;

    	// Right Arrow
    	case 39:
    		if(!checkCollisionRight()){
    			removeActive();
    			activeX++;
    			drawActive();
    		}
    		break;

    	// Down Arrow
    	case 40:
    		if(!checkCollisionDown()){
    			removeActive();
    			activeY++;
    			drawActive();
				score = score + 1;
    		}
    		break;

    	// Everything Else
    	default:
    		
    		break;
    }
	}
};

// Rotation

/*

var newField = [
					[tor[][],tor[][],tor[][],tor[][]],
					[tor[][],tor[][],tor[][],tor[][]],
					[tor[][],tor[][],tor[][],tor[][]],
					[tor[][],tor[][],tor[][],tor[][]]
				   ];

*/
function randomPiece(){
	switch (Math.floor(Math.random() * 7)) {
		case 0:
			return I_Piece;
		case 1:
			return J_Piece;
		case 2:
			return L_Piece;
		case 3:
			return O_Piece;
		case 4:
			return S_Piece;
		case 5:
			return T_Piece;
		case 6:
			return Z_Piece;
		default:
			return I_Piece;
	}
}

function rotateRight (tor) {
	var newField = [
					[tor[3][0],tor[2][0],tor[1][0],tor[0][0]],
					[tor[3][1],tor[2][1],tor[1][1],tor[0][1]],
					[tor[3][2],tor[2][2],tor[1][2],tor[0][2]],
					[tor[3][3],tor[2][3],tor[1][3],tor[0][3]]
				   ];

	return newField;			   
}

// Check Collision
function checkCollisionDown () {
	/*if(activeY == 16){
		lineClear();
		return true;
	}else{*/
		// New Check
		for(var y = 3, lengthY = -1; y > lengthY; y--){
			for(var x = 3, lengthX = -1;x > lengthX; x--){
				if(activePiece[y][x] > 0 && activeY > -3){
					try {
						//console.log(activeY+y+1)
						if(field[activeY+y+1][activeX+x] > 0){
							
							

							if(activeY < 0){
								gameOver();
								return false;
							}

							lineClear();
							return true;
						}
					} catch(e) {
						if(activeY >= 16){
							return true;
						}else{
							return false;	
						}
						
					}
					
				}
			}
		}

		// Old Check
		/*for(var x = 0, checkX = activePiece[3].length; x < checkX; x++){

			// Layer 3
			if(activePiece[3][x] > 0){
				if(field[activeY+4][activeX+x] > 0){
					if(activeY < 0){
						gameOver();
						return false;
					}
					lineClear();
					return true;
				}
			}

			// Layer 2
			if(activePiece[2][x] > 0 && activeY > -4){
				if(field[activeY+3][activeX+x] > 0){
					if(activeY < 0){
						gameOver();
						return false;
					}
					lineClear();
					return true;
				}
			}


			// Layer 1
			if(activePiece[1][x] > 0 && activeY > -3){
				if(field[activeY+2][activeX+x] > 0){
					if(activeY < 0){
						gameOver();
						return false;
					}
					lineClear();
					return true;
				}
			}

			// Layer 0
			if(activePiece[0][x] > 0 && activeY > -2){
				if(field[activeY+2][activeX+x] > 0){
					if(activeY < 0){
						gameOver();
						return false;
					}
					lineClear();
					return true;
				}
			}
		}*/
	//}
}

function checkCollisionRight() {
	for(var y = 3, lengthY = -1; y > lengthY; y--){
		for(var x = 3, lengthX = -1; x > lengthX; x--){
			if(activeX >= 6 && activePiece[y][x] > 0 && field[y+activeY][x+activeX+1] == undefined){
				return true;
			}
			if(activePiece[y][x] > 0 && activeY > 0){
				if(field[activeY+y][activeX+x+1] > 0){
					lineClear();
					return true;
				}
			}
		}
	}
}

function checkCollisionLeft () {
	/*if(activeX <= -1){
		lineClear();
		return true;
	}*/
	for(var y = 0, lengthY = 4; y < lengthY; y++){
		for(var x = 0, lengthX = 4; x < lengthX; x++){
			if(activeX <= 0 && activePiece[y][x] > 0 && field[y+activeY][x+activeX-1] == undefined){
				return true;
			}
			if(activePiece[y][x] > 0 && activeY > 0){
				if(field[y+activeY][x+activeX-1] > 0){
					lineClear();
					return true;
				}
			}
		}
	}
}

function lineClear(){
	var templineclear = 0;
	for(var y = 0, lengthY = field.length; y < lengthY; y++){
		if((field[y][0]*field[y][1]*field[y][2]*field[y][3]*field[y][4]*field[y][5]*field[y][6]*field[y][7]*field[y][8]*field[y][9]) > 0){
			templineclear++;
			linesCleared++;
			for(var i = 0, length1 = field[y].length; i < length1; i++){
				field[y][i] = 0;
			}

			for(var dropY = y, lengthDropY = 0; dropY > lengthDropY; dropY--){
				field[dropY] = field[dropY-1];
			}


			redrawField();
		}
	}
	switch(templineclear){
		case 1:
			score += 100 * level;
			break;
		case 2:
			score += 300 * level;
			break;
		case 3:
			score += 500 * level;
			break;
		case 4:
			score += 800 * level;
			break;
		default:
			break;
	}
}

// Draw Piece
function drawPiece (pieceX,pieceY,piece) {
	for(var y = 0, lengthY = piece.length; y < lengthY; y++){
		for(var x = 0, lengthX = piece[y].length; x < lengthX; x++){
			if(piece[y][x] > 0){
				ctx.fillStyle = getColor(piece[y][x]);
				ctx.fillRect(x+pieceX,y+pieceY,1,1);	
			}
		}
	}
}

// Draw Active
function drawActive () {
	for(var y = 0, lengthY = activePiece.length; y < lengthY; y++){
		for(var x = 0, lengthX = activePiece[y].length; x < lengthX; x++){
			if(activePiece[y][x] > 0){
				ctx.fillStyle = getColor(activePiece[y][x]);
				ctx.fillRect(x+activeX,y+activeY,1,1);
			}
		}
	}
}

// Remove Active Piece
function removeActive () {
	for(var y = 0, lengthY = activePiece.length; y < lengthY; y++){
		for(var x = 0, lengthX = activePiece[y].length; x < lengthX; x++){
			if(activePiece[y][x] > 0){
				ctx.clearRect(x+activeX,y+activeY,1,1);
				ctx.fillStyle = getColor(getColor(666));
				ctx.fillRect(x+activeX,y+activeY,1,1);	
			}
		}
	}
}

function putActive () {
	for(var y = 0, length1 = activePiece.length; y < length1; y++){
		for(var x = 0, length2 = activePiece[y].length; x < length2+1; x++){
			if(activePiece[y][x] > 0){
				if(activeY > -4){
					field[y+activeY][x+activeX] = activePiece[y][x];	
				}
			}
		}
	}
}

// Draw Square
function drawSquare (x,y,color) {
	ctx.clearRect(x,y,1,1);
	ctx.fillStyle = getColor(color);
	ctx.fillRect(x,y,1,1);
}

// Draw Next
function drawSquareNext (x,y,color) {
	ctx_next.clearRect(x,y,1,1);
	ctx_next.fillStyle = getColor(color);
	ctx_next.fillRect(x,y,1,1);
}

// Create Field and Get 2D Context from Canvas
function createField () {
	canvas = document.getElementById('playField'); // Load Canvas into canvas variable
	ctx = canvas.getContext('2d'); // Get the 2D context

	canvas_next = document.getElementById('game_content_right_next_canvas');
	ctx_next = canvas_next.getContext('2d');

	ctx.canvas.width = field_columns * block_size;
	ctx.canvas.height = field_rows * block_size;

	ctx_next.canvas.width = next_columns * block_size;
	ctx_next.canvas.height = next_rows * block_size;
	
	ctx.scale(block_size, block_size);
	ctx_next.scale(block_size,block_size);

	ctx_next.fillStyle = getColor(4);
	ctx_next.fillRect(0,0,1,1);

	if(debug_log){console.log('Game Field Created');}
}

// Redraw Field
function redrawField () {
	for(var y = 0, lengthY = field.length; y < lengthY; y++){
		for(var x = 0, lengthX = field[y].length; x < lengthX; x++){
			drawSquare(x,y,field[y][x])
		}
	}
}

// Redraw Next
function redrawNext(){
	for (let ny = 0; ny < 4; ny++) {
		for (let nx = 0; nx < 4; nx++) {
			drawSquareNext(nx,ny,nextPiece[ny][nx]);
		}
	}
}

// Utilities
// Function to get an empty field
function getEmptyField () {
	return Array.from(
      {length: field_rows}, () => Array(field_columns).fill(0)
    );
}

// Get Color
function getColor (color) {
	switch (color) {
		// I-Block
		case 1:
			return "#00FFFFFF";
			break;
		// J-Block
		case 2:
			return "#0000FFFF";
			break;
		// L-Block
		case 3:
			return "#FFBBBBFF";
			break;
		// O-Block
		case 4:
			return "#FFFF00FF";
			break;
		// S-Block
		case 5:
			return "#00FF00FF";
			break;
		// T-Block
		case 6:
			return "#FF00FFFF";
			break;
		// Z-Block
		case 7:
			return "#FF0000FF";
			break;
		case undefined:
			return "#CDAABBFF";
			break;
		// Default
		default:
			// return "#21212180";
			return "#00000000"
			break;
	}
}

// Pieces
const I_Piece = [[0,1,0,0],
				 [0,1,0,0],
				 [0,1,0,0],
				 [0,1,0,0]];

const J_Piece = [[0,0,0,0],
				 [0,0,2,0],
				 [2,2,2,0],
				 [0,0,0,0]];

const L_Piece = [[0,0,0,0],
			    [0,3,0,0],
 			    [0,3,3,3],
			    [0,0,0,0]];

const O_Piece = [[0,0,0,0],
			    [0,4,4,0],
			    [0,4,4,0],
			    [0,0,0,0]];

const S_Piece = [[0,0,0,0],
				 [0,5,5,0],
				 [5,5,0,0],
				 [0,0,0,0]]

const T_Piece = [[0,0,0,0],
				 [0,6,0,0],
				 [6,6,6,0],
				 [0,0,0,0]]

const Z_Piece = [[0,0,0,0],
				 [0,7,7,0],
				 [0,0,7,7],
				 [0,0,0,0]]