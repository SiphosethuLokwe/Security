using System.ComponentModel.DataAnnotations;

namespace Security.Models.DTO
{
    public class LoginModel
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
