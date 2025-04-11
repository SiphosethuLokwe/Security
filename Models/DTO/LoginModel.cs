using System.ComponentModel.DataAnnotations;

namespace Security.Models.DTO
{
    public class LoginModel
    {
        [Required]
        public required string Username { get; set; }
        [MinLength(6)]

        [Required]
        public required string Password { get; set; }
    }
}
