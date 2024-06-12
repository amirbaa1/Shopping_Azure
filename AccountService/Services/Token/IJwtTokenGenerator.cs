using AccountService.Model;

namespace AccountService.Services.Token;

public interface IJwtTokenGenerator
{
    string GeneratorToken(AppUser appUser);
}