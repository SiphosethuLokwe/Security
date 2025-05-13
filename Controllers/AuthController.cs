using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Security.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.Threading.Tasks;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IValidationService _validationService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, IValidationService validationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _validationService = validationService;
    }

    [HttpGet("external-login")]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            return BadRequest(new { Error = $"Error from external provider: {remoteError}" });
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return BadRequest(new { Error = "Error loading external login information." });
        }
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { Error = "Failed to create user." });
            }

            await _userManager.AddLoginAsync(user, info);
        }

        var token = _tokenService.GenerateToken(user);
        var refreshToken = await _tokenService.GenerateRefreshToken(user);

        return Ok(new { Token = token, RefreshToken = refreshToken.Token });
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var validationResult = await _validationService.ValidateRegisterModel(model).ConfigureAwait(false);
        if (!validationResult.Succeeded)
        {
            return BadRequest(validationResult.Errors);
        }

        var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "FrontEnd").ConfigureAwait(false);
            var token = _tokenService.GenerateToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user).ConfigureAwait(false);
            return Ok(new { Token = token, RefreshToken = refreshToken.Token });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var validationResult = await _validationService.ValidateLoginModel(model).ConfigureAwait(false);
        if (!validationResult.Succeeded)
        {
            return Unauthorized();
        }

        var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false).ConfigureAwait(false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
            var token = _tokenService.GenerateToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user).ConfigureAwait(false);
            return Ok(new { Token = token, RefreshToken = refreshToken.Token });
        }

        return Unauthorized();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshModel model)
    {
        var refreshToken = await _tokenService.GetRefreshToken(model.RefreshToken);

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.IsUsed || refreshToken.ExpiryDate <= DateTime.Now)
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(refreshToken.UserId).ConfigureAwait(false);
        var newToken = _tokenService.GenerateToken(user);
        var newRefreshToken = await _tokenService.GenerateRefreshToken(user).ConfigureAwait(false);

        refreshToken.IsUsed = true;
        await _tokenService.RevokeRefreshToken(refreshToken).ConfigureAwait(false);

        return Ok(new { Token = newToken, RefreshToken = newRefreshToken.Token });
    }
}
