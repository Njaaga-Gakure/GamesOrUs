using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.DTOs
{
    public class NewUserMessageDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

    }
}
