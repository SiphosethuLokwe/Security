using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class ApplicationUser : IdentityUser
{
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? Race { get; set; }
    public string? Gender { get; set; }
    public bool? HasDisability { get; set; }
    public bool? IsSouthAfricanCitizen { get; set; }

    public string? IdNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? IsMinor { get; set; }

    // Contact details (Identity already has PhoneNumber)
    public string? PhysicalAddress { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PostalCode { get; set; }

    public string? EmploymentStatus { get; set; }
    public string? Representing { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}