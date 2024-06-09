using Microsoft.AspNetCore.Identity;

namespace WebIdentityServer.Data;

public static class SeedUserData
{
    public static void Seed(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            if (context.Users.Count() == 0)
            {
                IdentityRole identityRoleAdmin = new IdentityRole
                {
                    Name = "Admin",
                };

                IdentityRole identityRoleCustomer = new IdentityRole
                {
                    Name = "Customer",
                };

                var rolecontext = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                rolecontext.CreateAsync(identityRoleAdmin).Wait();
                rolecontext.CreateAsync(identityRoleCustomer).Wait();


                foreach (var item in UserList())
                {
                    var result = context.CreateAsync(item, "Admin@123").Result;
                    if (item.UserName == "amirbaa")
                    {
                        var addToAdminRole = context.AddToRoleAsync(item, "Admin").Result;
                    }
                    else
                    {
                        var addToAdminRole = context.AddToRoleAsync(item, "Customer").Result;
                    }
                }
            }
        }
    }


    private static List<IdentityUser> UserList()
    {
        return new List<IdentityUser>()
        {
            new IdentityUser()
            {
                UserName = "amirba",
                Email = "a@b.com",
                EmailConfirmed = true,
            },
            new IdentityUser()
            {
                UserName = "amirab",
                Email = "amir@gmail.com",
                EmailConfirmed = true,
            },
        };
    }
}