using System.Collections.Generic;

namespace Tetros
{
    public class UserStorage
    {
        Dictionary<string, User> users;

        public UserStorage()
        {
            this.users = new Dictionary<string, User>();
        }

        public bool addUser(string username, string password, AccessLevel accessLevel)
        {
            if (users.ContainsKey(username))
            {
                return false;
            }
            else
            {
                string salt = Utilities.randomString(10);
                User temp = new User(username, Utilities.stringToHash(password + salt), salt, accessLevel);
                users.Add(username, temp);
                return true;
            }
        }

        public bool addUser(string username, string password)
        {
            if (users.ContainsKey(username))
            {
                return false;
            }
            else
            {
                string salt = Utilities.randomString(10);
                User temp = new(username, Utilities.stringToHash(password + salt), salt, AccessLevel.User);
                users.Add(username, temp);
                return true;
            }
        }

        public string getPasswordHash(string username)
        {
            return users[key: username].passwordHash;
        }

        public string getSalt(string username)
        {
            return users[key: username].passwordSalt;
        }

        public bool userExists(string username)
        {
            return users.ContainsKey(username);
        }

        public User getUser(string username)
        {
            return users[key: username];
        }
    }
}
