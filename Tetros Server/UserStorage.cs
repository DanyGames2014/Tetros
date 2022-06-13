using System.Collections.Generic;

namespace Tetros
{
    /// <summary>
    /// Class used to store users
    /// </summary>
    public class UserStorage
    {
        public Dictionary<string, User> users;

        public UserStorage()
        {
            this.users = new Dictionary<string, User>();
        }

        /// <summary>
        /// Adds a user with the defined username, password and access Level.
        /// Password Hash and Salt is calculated automatically.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="accessLevel">Access Level</param>
        /// <returns>Returns if the action was successful.</returns>
        public bool addUser(string username, string password, AccessLevel accessLevel)
        {
            lock (users)
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
        }

        /// <summary>
        /// Adds a user with the defined username, password.
        /// Access Level Defaults to User
        /// Password Hash and Salt is calculated automatically.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Returns if the action was successful.</returns>
        public bool addUser(string username, string password)
        {
            lock (users)
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
        }

        /// <summary>
        /// Adds already created user.
        /// Checks if the defined username doesn't already exist.
        /// </summary>
        /// <param name="user">User To Add</param>
        /// <returns>Returns if the action was successful.</returns>
        public bool addUser(User user)
        {
            lock (users)
            {
                if (users.ContainsKey(user.username))
                {
                    return false;
                }
                else
                {
                    users.Add(user.username, user);
                    return true;
                }
            }
        }

        /// <summary>
        /// Gets the password hash of the user using his username.
        /// </summary>
        /// <returns>Password Hash</returns>
        public string getPasswordHash(string username)
        {
            lock (users)
            {
                return users[key: username].passwordHash;
            }
        }

        /// <summary>
        /// Gets the password salt of the user using his username.
        /// </summary>
        /// <returns>Password Salt</returns>
        public string getSalt(string username)
        {
            lock (users)
            {
                return users[key: username].passwordSalt;
            }
        }

        /// <summary>
        /// Returns if a user with the defined username exists.
        /// </summary>
        public bool userExists(string username)
        {
            lock (users)
            {
                return users.ContainsKey(username);
            }
        }

        /// <summary>
        /// Returns the User object according to the given username.
        /// </summary>
        public User getUser(string username)
        {
            lock (users)
            {
                return users[key: username];
            }
        }
    }
}
