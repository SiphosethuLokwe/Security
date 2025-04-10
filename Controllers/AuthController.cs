using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Security.Models.DTO;
using System.Threading.Tasks;

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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        try
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
                await _userManager.AddToRoleAsync(user, "Admin").ConfigureAwait(false);
                var token = _tokenService.GenerateToken(user);
                var refreshToken = await _tokenService.GenerateRefreshToken(user).ConfigureAwait(false);
                return Ok(new { Token = token, RefreshToken = refreshToken.Token });
            }

            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshModel model)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}