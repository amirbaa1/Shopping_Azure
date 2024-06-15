using Microsoft.AspNetCore.Identity;

namespace AccountService.Model;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Image { get; set; }
}