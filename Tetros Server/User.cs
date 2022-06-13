namespace Tetros
{
    public enum AccessLevel
    {
        User = 0,
        Admin = 1
    }

    /// <summary>
    /// User Object
    /// </summary>
    public class User
    {
        public string username;
        public string passwordHash;
        public string passwordSalt;
        public AccessLevel level;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="passwordHash">Hashed Password+Salt of the user</param>
        /// <param name="passwordSalt">Salt of the user's password</param>
        /// <param name="level">Access Level of this User</param>
        public User(string username, string passwordHash, string passwordSalt, AccessLevel level)
        {
            this.username = username;
            this.passwordHash = passwordHash;
            this.passwordSalt = passwordSalt;
            this.level = level;
        }

    }
}
