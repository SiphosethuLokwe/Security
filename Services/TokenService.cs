using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _configuration = configuration;
        _userManager = userManager;
        _context = context;
    }

    public string GenerateToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("UserId", user.Id),
            new Claim("Email", user.Email ?? ""),
            new Claim("PhoneNumber", user.PhoneNumber ?? ""),
            
            // Personal info
            new Claim("Title", user.Title ?? ""),
            new Claim("FirstName", user.FirstName ?? ""),
            new Claim("LastName", user.LastName ?? ""),
            new Claim("Race", user.Race ?? ""),
            new Claim("Gender", user.Gender ?? ""),
            new Claim("HasDisability", user.HasDisability?.ToString() ?? ""),
            new Claim("IsSouthAfricanCitizen", user.IsSouthAfricanCitizen?.ToString() ?? ""),
            
            // Identity
            new Claim("IdNumber", user.IdNumber ?? ""),
            new Claim("DateOfBirth", user.DateOfBirth?.ToString("O") ?? ""),
            new Claim("IsMinor", user.IsMinor?.ToString() ?? ""),
            
            // Contact / Address
            new Claim("PhysicalAddress", user.PhysicalAddress ?? ""),
            new Claim("Province", user.Province ?? ""),
            new Claim("City", user.City ?? ""),
            new Claim("District", user.District ?? ""),
            new Claim("PostalCode", user.PostalCode ?? ""),
            
            // Employment & representation
            new Claim("EmploymentStatus", user.EmploymentStatus ?? ""),
            new Claim("Representing", user.Representing ?? "")
        };

        var roles = _userManager.GetRolesAsync(user).Result;
        if (roles.Any())
        {
            claims.Add(new Claim("Roles", JsonSerializer.Serialize(roles.ToList())));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshToken> GenerateRefreshToken(ApplicationUser user)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            ExpiryDate = DateTime.Now.AddDays(7),
            IsRevoked = false,
            IsUsed = false,
            User = user // Set the required User property
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync().ConfigureAwait(false);

        return refreshToken;
    }

    public async Task<RefreshToken> GetRefreshToken(string token)
    {
        return await _context.RefreshTokens.SingleOrDefaultAsync(rt => rt.Token == token).ConfigureAwait(false);
    }

    public async Task RevokeRefreshToken(RefreshToken refreshToken)
    {
        refreshToken.IsRevoked = true;
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync().ConfigureAwait(false);
    }
}