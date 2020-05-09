using System;

namespace BCI.SLAPS.Server.Model.Chat
{
    public class Post
    {
        /// <summary>
        /// Unique identifier for this post
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Timestamp the post was created
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Contains the user object if the post was submitted by an authenticated User.
        /// </summary>
        public User.User User { get; set; }

        /// <summary>
        /// The name of the message author
        /// </summary>
        public string Author 
        {
            get
            {
                if (User != null)
                    return User.Username;
                else return _Author;
            }
            set => _Author = value;
        }
        private string _Author;

        /// <summary>
        /// Contents of the message
        /// </summary>
        public string Message { get; set; }
    }
}
