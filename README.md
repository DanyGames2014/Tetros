# Tetros
 
Tetris style game with frontend in HTML/CSS/JS and backend in C#

## How To Setup - Release
### Server
1. Download The Latest Release of TetrosServer.zip from Releases.
2. Extract it to a directory where you want the serve to be located.
3. (Optional) Open Tetros.dll.config and Configure Port, Session Validity, Logging and Userdata Storage according to your needs.
4. Run Tetros.exe and if prompted allow it in Firewall.
5. On first Run the server will generates "Logs" and "Userdata" folder. (These names may be different if you configured them)
6. The server is now listening for connections from clients.

### Client - An HTTP Server is needed for this.
1. Download The Latest Release of TetrosClient.zip from Releases.
2. Extract the contents of TetrosClient.zip into your HTTP Server Directory.
3. Point your HTTP Server to index.html.
4. Navigate to the script folder and open websocket.js in a text editor.
5. On line 12 change the ``` ws://localhost:80 ``` address to point to the Tetros Server with the port you set. (Default Port is 80)
6. Now when index.html is opened it will try to estabilish a connection to the defined server.

## Options in ```Tetros.dll.config```
**ServerPort** - This is the Port the Server will be Listening On

**SessionValidSeconds** - Defines for how many seconds a session will be kept valid when a user logs in.

**ThreadCountCleanup** - Defines how many threads can be active before Stopped threads are Purged.

**LoggingEnabled** - If true, the server will also log the console output to a file.

**LoggingDirectory** - Name of the directory to store log files.

**UserdataStorageEnabled** - If Userdata should be saved/loaded. If disabled everything will be lost when server is turned off. (NYI)

**UserdataStorageMethod** - Where Userdata will be stored. Avalible Options : ```File```

**UserdataStorageDirectory** - Name of the directory to store Userdata.

**AutosaveInterval** - Defined the interval in minutes between autosaves.
