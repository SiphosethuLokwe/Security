
public interface ITokenService
{
    Task<RefreshToken> GenerateRefreshToken(ApplicationUser user);
    string GenerateToken(ApplicationUser user);
    Task<RefreshToken> GetRefreshToken(string token);
    Task RevokeRefreshToken(RefreshToken refreshToken);
}