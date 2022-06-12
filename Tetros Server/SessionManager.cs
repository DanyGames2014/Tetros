using System;
using System.Collections.Generic;
using System.Configuration;

namespace Tetros
{
    public class SessionManager
    {
        public Dictionary<string, Session> sessions;

        public SessionManager()
        {
            sessions = new Dictionary<string, Session>();
        }

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
