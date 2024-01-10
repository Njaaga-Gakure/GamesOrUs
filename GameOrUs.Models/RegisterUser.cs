using System.ComponentModel.DataAnnotations;

namespace GameOrUs.Models
{
    public class RegisterUser
    { 

        [Required, MinLength(3)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MinLength(3)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(10)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
