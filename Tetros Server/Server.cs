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


        public Server()
        {
            userStorage = new UserStorage();
            authManager = new(userStorage);
            sessionManager = new SessionManager();

            logger = new Logger();
            
            IPAddress IPAddress = Dns.GetHostEntry("localhost").AddressList[0];
            tcpListener = new TcpListener(IPAddress.Any, 80);

            authManager.userStorage.addUser("pablo", "pablo", AccessLevel.Admin);

            threadCountCleanup = Convert.ToInt32(ConfigurationManager.AppSettings.Get("ThreadCountCleanup"));

        }

        public void Start()
        {
            logger.WriteInfo("Server Started on Port " + 80);
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
                    logger.WriteInfo("Thread Count Éxceeded Cleanup Threshold(" + threadCountCleanup + "), Cleaning Stopped Threads");
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

                /*foreach (var item in handlers)
                {
                    

                    logger.WriteLine(
                        "Thread ID : " + item.ManagedThreadId + " | " +
                        "Thread Name : " + item.Name + " | " +
                        "Thread Status : " + item.ThreadState
                        ,ConsoleColor.Cyan);
                }
                */
            }
        }

        public void connectionChange(int change)
        {
            lock (this)
            {
                activeConnections += change;
            }
        }
    }
}
