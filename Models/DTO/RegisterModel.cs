using System.ComponentModel.DataAnnotations;

namespace Security.Models.DTO
{
    public class RegisterModel
    {
        // Login / Security
        [Required]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        // Basic Personal Info
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        public string? Title { get; set; }  // Mr, Ms, etc.
        public string? Gender { get; set; }
        public string? Race { get; set; }

        public bool? HasDisability { get; set; }
        public bool? IsSouthAfricanCitizen { get; set; }

        // Legal Identity
        [Required]
        public required string IdNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public bool? IsMinor { get; set; }

        // Contact details
        public string? PhoneNumber { get; set; }

        // Address
        public string? PhysicalAddress { get; set; }
        public string? Province { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? PostalCode { get; set; }

        // Employment & Representation
        public string? EmploymentStatus { get; set; } // Unemployed, Employed, etc.
        public string? Representing { get; set; } // Self / Team / Company
    }
}
