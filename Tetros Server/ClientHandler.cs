using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Tetros
{
    class ClientHandler
    {
        // Connection Info
        public int connectionID;
        public Session session;
        public DisconnectReason disconnectReason;

        // Temp
        public bool run = true;
        public bool handshakeDone = false;
        public HandshakeStage handshakeStage = HandshakeStage.ReceiveHandshake;
        public Dictionary<string, string> handshake;
        Server server;
        SessionManager sessionManager;

        // Network Stuff
        TcpClient tcpClient;
        Socket socket;
        NetworkStream ns;
        StreamReader sr;
        StreamWriter sw;

        // Logger
        Logger logger;


        public ClientHandler(TcpClient client, int ConnectionID, Server server)
        {
            disconnectReason = DisconnectReason.Unknown;

            this.server = server;

            sessionManager = server.sessionManager;

            this.connectionID = ConnectionID;

            this.tcpClient = client;
            this.socket = tcpClient.Client;

            ns = tcpClient.GetStream();
            sr = new(ns);
            sw = new(ns);

            sw.AutoFlush = true;

            handshake = new Dictionary<string, string>();

            logger = server.logger;

            logger.WriteInfo("Session initiated. Connection ID : " + ConnectionID);
        }

        public void writeToNs(string text)
        {
            byte[] towrite = Utilities.encodeTextFrame(text);
            ns.Write(towrite, 0, towrite.Length);
        }

        public void ConnectionHandler()
        {
            try
            {
                while (run)
                {

                    logger.WriteInfo("Attempting Handshake");
                    while (handshakeDone == false)
                    {
                        switch (handshakeStage)
                        {
                            case HandshakeStage.ReceiveHandshake: // STAGE 1
                                string temp = sr.ReadLine();
                                if (string.IsNullOrEmpty(temp))
                                {
                                    handshakeStage = HandshakeStage.AssembleResponse;
                                }
                                else
                                {

                                    string[] testos = temp.Trim().Split(":");
                                    switch (testos.Length)
                                    {
                                        case 1: // GET
                                            if (testos[0].Contains("GET"))
                                            {
                                                handshake.Add("GET", testos[0].Trim());
                                            }
                                            break;

                                        case 2: // Normal
                                            handshake.Add(testos[0].Trim(), testos[1].Trim());
                                            break;

                                        case 3: // IP
                                            handshake.Add(testos[0].Trim(), testos[1].Trim() + ":" + testos[2].Trim());
                                            break;

                                        default:
                                            break;
                                    }

                                }
                                break;

                            case HandshakeStage.AssembleResponse: // STAGE 2
                                string response = "" +
                                    "HTTP/1.1 101 Switching Protocols\r\n" +
                                    "Upgrade: websocket\r\n" +
                                    "Connection: Upgrade\r\n" +
                                    "Sec-WebSocket-Accept: " + Utilities.ServerWebsocketKey(handshake[key: "Sec-WebSocket-Key"]) + "\r\n" +
                                    "";
                                sw.WriteLine(response);
                                handshakeStage = HandshakeStage.SendResponse;
                                break;

                            case HandshakeStage.SendResponse:    // STAGE 3
                                handshakeDone = true;
                                break;
                        }
                    }

                    logger.WriteInfo("Handshake Done");
                    while (handshakeDone)
                    {
                        if (ns.DataAvailable)
                        {
                            bool decoded = false;

                            // Creates a Field with the size of avalible Data
                            Byte[] bytes = new Byte[tcpClient.Available];

                            // Reads all the bytes
                            ns.Read(bytes, 0, bytes.Length);

                            // Decodes them into Frame type
                            Frame frame = Utilities.decodeFrame(bytes);

                            // Decode Frame data according to the opcode
                            switch (frame.opcode)
                            {
                                case 1:  // Text Frame

                                    // Processing of the data
                                    Dictionary<string, string> frameData = new();
                                    string[] tempFrameDataLines = Encoding.UTF8.GetString(frame.payloadData)
                                        .Replace(" ", "")
                                        .Trim()
                                        .Split("\n", StringSplitOptions.TrimEntries);

                                    foreach (var item in tempFrameDataLines)
                                    {
                                        string[] tempFrameDataLine = item.Split(":");
                                        if (tempFrameDataLine.Count() == 2)
                                        {
                                            frameData.Add(tempFrameDataLine[0].Trim(), tempFrameDataLine[1].Trim());
                                            decoded = true;
                                        }
                                        else
                                        {
                                            logger.WriteWarn(100);
                                            decoded = false;
                                            //throw new InvalidFrameDataFormatException();
                                        }

                                    }

                                    if (decoded)
                                    {
                                        switch (frameData[key: "type"])
                                        {
                                            case "RANDOM":
                                                writeToNs("RANDOM:" + Utilities.randomString(10));
                                                break;

                                            case "SESSIONGET":
                                                Session sessionGET = sessionManager.getSessionWithKey(frameData["sessionKey"]);
                                                if(sessionGET != null)
                                                {
                                                    session = sessionGET;
                                                    writeToNs("SESSIONGET:" + 
                                                        "true;" + session.sessionUser.username);
                                                    logger.WriteInfo("User " + session.sessionUser.username + " retrieved session. Connection ID : " + connectionID);
                                                }
                                                else
                                                {
                                                    writeToNs("SESSIONGET:" + "false");
                                                }
                                                break;

                                            case "LOGIN":
                                                bool auth = server.authManager.authUser(frameData["username"], frameData["password"]);
                                                if (auth)
                                                {
                                                    string NewSessionKey = Utilities.randomString(50);
                                                    Session newSession = new Session(NewSessionKey, server.userStorage.getUser(frameData["username"]));
                                                    sessionManager.createSession(newSession);
                                                    session = newSession;
                                                    writeToNs("AUTHRESULT:" +
                                                        "true;" + frameData["username"] + ";" + NewSessionKey);
                                                    logger.WriteInfo("User " + frameData["username"] + " logged in. Connection ID : " + connectionID);
                                                }
                                                else
                                                {
                                                    writeToNs("AUTHRESULT:false");
                                                }
                                                break;

                                            case "LOGOUT":
                                                bool logout_result = sessionManager.invalidateSessionKey(session.sessionKey);
                                                writeToNs("LOGOUT:" + logout_result);
                                                logger.WriteInfo("User Logged Out, Invalidating Session Key");
                                                session = null;
                                                break;

                                            case "SCORESUBMIT":
                                                try
                                                {
                                                    User score_user = server.userStorage.getUser(frameData["username"]);
                                                    server.leaderboard.submitScore(Convert.ToInt32(frameData["score"]), score_user);
                                                }
                                                catch (Exception)
                                                {
                                                    writeToNs("ERROR:UnknownUser");
                                                }
                                                break;

                                            default:
                                                writeToNs("ERROR:UnknownCommand");
                                                break;
                                        }
                                    }
                                    break;

                                case 8:  // Close Frame
                                    //Console.WriteLine("Received Close Frame, Closing Connection");
                                    disconnectReason = DisconnectReason.UserDisconnect;
                                    handshakeDone = false;
                                    run = false;
                                    break;

                                case 9:  // Ping
                                    logger.WriteInfo("Received Ping, Sending Pong");
                                    byte[] response_pong = Utilities.encodePongFrame();
                                    ns.Write(response_pong, 0, response_pong.Length);
                                    break;

                                default:
                                    throw new UnknownOpcodeException();

                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                logger.WriteError("Error in Session " + connectionID, e);
                ns.Close();
            }
            finally
            {
                server.connectionChange(-1);
                logger.WriteInfo("User Disconnected | Reason : " + disconnectReason + " | Connection ID: " + connectionID);
                //logger.WriteWarn("Active Connections : " + server.activeConnections);
                //logger.WriteLine(Thread.CurrentThread.ManagedThreadId+"", ConsoleColor.Green);
            }
        }
    }
}
