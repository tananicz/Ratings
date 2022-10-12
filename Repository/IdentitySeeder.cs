using Microsoft.AspNetCore.Identity;

namespace Ratings.Repository
{
    public class IdentitySeeder
    {
        public static async Task SeedDatabase(UserManager<IdentityUser> userMgr, RoleManager<IdentityRole> roleMgr, IConfiguration configuration, ILogger<IdentitySeeder> logger)
        {
            if (!userMgr.Users.Any() && !roleMgr.Roles.Any())
            {
                logger.LogInformation("Identity database empty, adding admin account and application roles");

                string userName = configuration["AdminData:UserName"];
                string passowrd = configuration["AdminData:Password"];

                if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(passowrd))
                {
                    await roleMgr.CreateAsync(new IdentityRole("admin"));
                    await roleMgr.CreateAsync(new IdentityRole("moderator"));
                    await roleMgr.CreateAsync(new IdentityRole("user"));

                    IdentityUser adminUser = new IdentityUser(userName);
                    await userMgr.CreateAsync(adminUser, passowrd);

                    await userMgr.AddToRoleAsync(adminUser, "admin");

                    logger.LogInformation("Identity database created");
                }
                else
                {
                    throw new Exception("Admin credentials missing");
                }
            }
        }
    }
}
