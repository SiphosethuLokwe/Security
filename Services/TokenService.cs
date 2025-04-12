using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };


        var roles = _userManager.GetRolesAsync(user).Result;
        claims.AddRange(roles.Select(role => new Claim("Roles", role)));

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