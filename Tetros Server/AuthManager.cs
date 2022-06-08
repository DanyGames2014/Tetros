namespace Tetros
{
    public class AuthManager
    {
        public UserStorage userStorage;

        public AuthManager(UserStorage userStorage)
        {
            this.userStorage = userStorage;
        }

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

        public AccessLevel getAccessLevel(string username)
        {
            return userStorage.getUser(username).level;
        }
    }
}
