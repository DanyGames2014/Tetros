namespace Tetros
{
    /// <summary>
    /// Class used to check credentials from a defined userStorage
    /// </summary>
    public class AuthManager
    {
        public UserStorage userStorage;

        /// <summary>
        /// Constructor
        /// </summary>
        public AuthManager(UserStorage userStorage)
        {
            this.userStorage = userStorage;
        }

        /// <summary>
        /// Checks Credentials agains userStorage
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Returns true if the credentials are correct and false if they aren't or the user doesn't exist</returns>
        public bool authUser(string username, string password)
        {
            if (userStorage.userExists(username))
            {
                string salt = userStorage.getSalt(username);
                if (userStorage.getPasswordHash(username).Equals(Utilities.stringToHash(password + salt)))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Gets the Access Level of the defined user
        /// </summary>
        /// <param name="username">username of the user to check</param>
        /// <returns>Access Level of that user</returns>
        public AccessLevel getAccessLevel(string username)
        {
            return userStorage.getUser(username).level;
        }
    }
}
