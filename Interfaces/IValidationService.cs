using Microsoft.AspNetCore.Identity;
using Security.Models.DTO;

public interface IValidationService
{
    Task<SignInResult> ValidateLoginModel(LoginModel model);
    Task<IdentityResult> ValidateRegisterModel(RegisterModel model);
}