using System.ComponentModel.DataAnnotations;


namespace GameOrUs.Models
{
    public class LoginUser
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
