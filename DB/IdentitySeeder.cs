using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

public static class IdentitySeeder
{
    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var users = new[]
        {
            new { UserName = "Forntend", Email = "user1@example.com", Password = "Pass@word1" },
            new { UserName = "admin", Email = "user2@example.com", Password = "Pass@word1" }
        };

        foreach (var u in users)
        {
            if (await userManager.FindByNameAsync(u.UserName) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, u.Password);
            }
        }
    }
}
