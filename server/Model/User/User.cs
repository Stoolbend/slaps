using BCI.SLAPS.Server.Helpers;
using System;

namespace BCI.SLAPS.Server.Model.User
{
    public class User
    {
        /// <summary>
        /// Unique identifier representing this object
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Base64 string used for creating the password hash.
        /// </summary>
        public byte[] Salt { get; set; }

        /// <summary>
        /// A Base64 string HMAC-SHA1 hash of the users password.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Indicates if the object is Historic or not.
        /// A value of True indicates that the object has been 'deleted' and should not be used.
        /// </summary>
        public bool Historic { get; set; }

        /// <summary>
        /// Check if the provided password matches the current User object's password.
        /// </summary>
        /// <param name="password">Password string to test</param>
        /// <returns>True if hash matches, false otherwise</returns>
        public bool CheckPassword(string password)
        {
            if (Salt != null && Hash != null)
                return Hash.Equals(PasswordUtils.GenerateHash(password, Salt), StringComparison.InvariantCulture);
            else return false;
        }
    }
}
