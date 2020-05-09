using System;
using System.ComponentModel.DataAnnotations;

namespace BCI.SLAPS.Server.Model.User
{
    #region Existing users
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
    }

    public class UserLoginResponseDTO : UserDTO
    { 
        public string Token { get; set; }
    }
    #endregion

    #region User registration
    public class UserLoginRequestDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserUpdateDTO
    {
        [Required]
        public string Password { get; set; }
    }
    #endregion
}
