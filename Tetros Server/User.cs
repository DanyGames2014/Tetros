namespace Tetros
{
    public enum AccessLevel
    {
        User = 0,
        Admin = 1
    }

    public class User
    {
        public string username;
        public string passwordHash;
        public string passwordSalt;
        public AccessLevel level;

        public User(string username, string passwordHash, string passwordSalt, AccessLevel level)
        {
            this.username = username;
            this.passwordHash = passwordHash;
            this.passwordSalt = passwordSalt;
            this.level = level;
        }

    }
}
