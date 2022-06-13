using System;
using System.Collections.Generic;
using System.Configuration;

namespace Tetros
{
    /// <summary>
    /// Class used to manage Sessions
    /// </summary>
    public class SessionManager
    {
        public Dictionary<string, Session> sessions;

        public SessionManager()
        {
            sessions = new Dictionary<string, Session>();
        }

        /// <summary>
        /// Creates a session and gives it expiration with the defined expiration in App.config
        /// </summary>
        public void createSession(Session session)
        {
            int validity = 0;
            try
            {
                validity = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SessionValidSeconds"));
                
            }
            catch (Exception)
            {
                Console.Error.WriteLine("There was an Error reading the App.config value 'SessionValidSeconds'." +
                    "Sessions won't be kept valid for any amount of time until the problem is resolved.");
                throw;
            }
            long expiration = DateTimeOffset.Now.ToUnixTimeSeconds() + validity;
            session.expiration = expiration;
            sessions.Add(session.sessionKey, session);
        }

        /// <summary>
        /// Creates a session with given key and user and gives it expiration with the defined expiration in App.config
        /// </summary>
        public void createSession(string sessionKey, User user)
        {
            int validity = 0;
            try
            {
                validity = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SessionValidSeconds"));
            }
            catch (Exception)
            {
                Console.Error.WriteLine("There was an Error reading the App.config value 'SessionValidSeconds'." +
                    "Sessions won't be kept valid for any amount of time until the problem is resolved.");
                throw;
            }
            

            long expiration = DateTimeOffset.Now.ToUnixTimeSeconds() + validity;
            Session session = new Session(sessionKey, user, expiration);
            sessions.Add(sessionKey, session);
        }

        /// <summary>
        /// Retrieves a session with the given key
        /// </summary>
        /// <param name="key">Session Key</param>
        public Session getSessionWithKey(string key)
        {
            long timenow = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (sessions.ContainsKey(key))
            {
                if(sessions[key].expiration < timenow)
                {
                    //Console.WriteLine("Removing Expired Session");
                    sessions.Remove(key);
                    return null;
                }
                else
                {
                    return sessions[key];
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines if user has an active Session
        /// </summary>
        public bool userHasSession(string username)
        {
            foreach (var item in sessions)
            {
                if (item.Value.sessionUser.username.Equals(username))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Invalidates all sessions this user has
        /// </summary>
        public bool invalidateSessionForUser(string username)
        {
            bool invalidated = false;
            foreach(var item in sessions)
            {
                if(item.Value.sessionUser.username == username)
                {
                    sessions.Remove(item.Key);
                    invalidated = true;
                }
            }
            return invalidated;
        }

        /// <summary>
        /// Invalidates a session with the defined key
        /// </summary>
        public bool invalidateSessionKey(string key)
        {
            if (sessions.ContainsKey(key))
            {
                sessions.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
