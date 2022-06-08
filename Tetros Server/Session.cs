namespace Tetros
{
    public class Session
    {
        public string sessionKey;
        public User sessionUser;
        public long expiration;

        public Session(string sessionKey, User sessionUser, long expiration)
        {
            this.sessionKey = sessionKey;
            this.sessionUser = sessionUser;
            this.expiration = expiration;
        }

        public Session(string sessionKey, User sessionUser)
        {
            this.sessionKey = sessionKey;
            this.sessionUser = sessionUser;
            expiration = 0;
        }
    }
}
