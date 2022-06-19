using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginSuccess
    {
        public User User { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string UserId { get; set; }

        public string RefreshToken { get; set; }
    }

    public class RefreshTokenSuccess
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

    public class Register
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
