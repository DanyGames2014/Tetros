using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Tetros
{
    public class Server
    {
        // TCP Handling
        TcpListener tcpListener;
        List<Thread> handlers = new List<Thread>();
        int port;

        // Managers
        public UserStorage userStorage;
        public AuthManager authManager;
        public SessionManager sessionManager;

        // Connection ID Tracking
        int connectionID = 0;

        // Stats
        public int activeConnections = 0;

        // Logger
        public Logger logger;

        // Leaderboard
        public Leaderboard leaderboard = new();

        // Temp
        public int threadCountCleanup = 5;
        public int autosaveMin = 5;


        /// <summary>
        /// Constructor for the Server
        /// Everything is initialized here and config values are read from App.config
        /// </summary>
        public Server()
        {
            // Initialize UserStorage, AuthManager and SessionManager
            userStorage = new UserStorage();
            authManager = new(userStorage);
            sessionManager = new SessionManager();

            // Initiliaze Logger
            logger = new Logger();

            // Attempt to read the port from App.config
            try
            {
                port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ServerPort"));

                if (port > 65535 || port < 1)
                {
                    logger.WriteError("Port is outside of the TCP Port Range");
                    Environment.Exit(1);
                }
            }
            catch (Exception e)
            {
                logger.WriteError("Invalid Value ServerPort in App.config", e);
                logger.WriteWarn("Using default port 80");
                port = 80;
            }

            // Initialize the TCP Listener
            IPAddress IPAddress = Dns.GetHostEntry("localhost").AddressList[0];
            tcpListener = new TcpListener(IPAddress.Any, port);

            // Set the amount of threads that will cause thread cleanup
            try
            {
                threadCountCleanup = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ThreadCountCleanup"));
            }
            catch (Exception e)
            {
                logger.WriteError("There was an error reading the ThreadCountCleanup value from App.config", e);
            }
            

            // Set the autosaveMin value from App.config
            try
            {
                autosaveMin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("AutosaveInterval"));
            }
            catch (Exception e)
            {
                logger.WriteError("There was an error reading the AutosaveInterval value from App.config", e);
                autosaveMin = 5;
            }

            // Temp User
            
            /*authManager.userStorage.addUser("pablo", "pablo", AccessLevel.Admin);
            authManager.userStorage.addUser("jakub", "aaaaa", AccessLevel.Admin);
            authManager.userStorage.addUser("pavel", "pasrrsrsblo", AccessLevel.Admin);
            authManager.userStorage.addUser("pepa", "pablfsdfsdfsdfo", AccessLevel.Admin);
            authManager.userStorage.addUser("excelsior", "gdgdgdgdfg", AccessLevel.Admin);

            leaderboard.submitScore(646846, "pablo", this);
            leaderboard.submitScore(44555, "pablo", this);
            leaderboard.submitScore(65454846, "pablo", this);
            leaderboard.submitScore(64545846, "pablo", this);
            */
            
        }

        /// <summary>
        /// Start method that listens to incoming connections and for each incoming connection makes a new thread
        /// </summary>
        public void Start()
        {
            Thread saveThread = new Thread(Save);
            DataSave.Load(this);

            logger.WriteInfo("Server Started on Port " + port);
            saveThread.Start();
            while (true)
            {
                tcpListener.Start();
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                //Socket serverSocket = tcpListener.AcceptSocket();
                logger.WriteInfo("Client Connected, Creating a Thread");

                ClientHandler temp = new ClientHandler(tcpClient, connectionID, this);
                handlers.Add(new Thread(temp.ConnectionHandler));

                handlers[handlers.Count - 1].Name = "Connection ID : " + connectionID;
                handlers[handlers.Count - 1].Start();
                connectionChange(1);
                connectionID++;

                if(handlers.Count > threadCountCleanup)
                {
                    logger.WriteInfo("Thread Count Exceeded Cleanup Threshold(" + threadCountCleanup + "), Cleaning Stopped Threads");
                    for (int i = 0; i < handlers.Count-1; i++)
                    {
                        if (handlers[i].ThreadState == ThreadState.Stopped)
                        {
                            handlers.RemoveAt(i);
                            i--;
                        }
                    }
                    logger.WriteInfo("New Thread Count : " + handlers.Count);
                }
            }
        }

        /// <summary>
        /// Method that is triggered every x minutes and causes an autosave
        /// </summary>
        public void Save()
        {
            Thread.Sleep(5000);
            while (true)
            {
                logger.WriteInfo("Autosave");
                DataSave.Save(this);
                Thread.Sleep(6000  * autosaveMin);
                
            }
        }

        /// <summary>
        /// Thread safe method to keep track of how many connections are active.
        /// </summary>
        /// <param name="change">Value to add to tthe activeConnections statistic</param>
        public void connectionChange(int change)
        {
            lock (this)
            {
                activeConnections += change;
            }
        }
    }
}
