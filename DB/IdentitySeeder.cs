using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var users = new[]
        {
        new { UserName = "Frontend", Email = "user1@example.com", Password = "Pass@word1", Roles = "Frontend" },
        new { UserName = "Admin", Email = "user2@example.com", Password = "Pass@word1", Roles = "Admin,Frontend" },
        new { UserName = "Applicant", Email = "applicant@example.com", Password = "Pass@word1", Roles = "Applicant" }
    };

        foreach (var u in users)
        {
            // Split roles by comma and trim whitespace
            var roleList = u.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Ensure roles exist
            foreach (var role in roleList)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Check if user exists
            var user = await userManager.FindByNameAsync(u.UserName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, u.Password);
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create user {u.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            // Assign roles
            foreach (var role in roleList)
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }

}
