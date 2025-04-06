using Microsoft.AspNetCore.Identity;
using Security.Models.DTO;

public class ValidationService : IValidationService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ValidationService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> ValidateRegisterModel(RegisterModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
        {
            return IdentityResult.Failed(new IdentityError { Description = "All fields are required." });
        }

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Username is already taken." });
        }

        var emailUser = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (emailUser != null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Email is already taken." });
        }

        return IdentityResult.Success;
    }

    public async Task<SignInResult> ValidateLoginModel(LoginModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        {
            return SignInResult.Failed;
        }

        var user = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(false);
        if (user == null)
        {
            return SignInResult.Failed;
        }

        return await _userManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false) ? SignInResult.Success : SignInResult.Failed;
    }
}