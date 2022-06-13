namespace Tetros
{
    /// <summary>
    /// Session Object
    /// </summary>
    public class Session
    {
        public string sessionKey;
        public User sessionUser;
        public long expiration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sessionKey">Also stored on the browser side and is used to determine which session to retrieve.</param>
        /// <param name="sessionUser">User to which the Session belongs to</param>
        /// <param name="expiration">Unix Timestamp of when the session expires</param>
        public Session(string sessionKey, User sessionUser, long expiration)
        {
            this.sessionKey = sessionKey;
            this.sessionUser = sessionUser;
            this.expiration = expiration;
        }

        /// <summary>
        /// Constructor of Session with Instant Expiration
        /// </summary>
        /// <param name="sessionKey">Also stored on the browser side and is used to determine which session to retrieve.</param>
        /// <param name="sessionUser">User to which the Session belongs to</param>
        public Session(string sessionKey, User sessionUser)
        {
            this.sessionKey = sessionKey;
            this.sessionUser = sessionUser;
            expiration = 0;
        }
    }
}
