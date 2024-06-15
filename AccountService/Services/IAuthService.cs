using AccountService.Model;
using AccountService.Model.Dto;

namespace AccountService.Services;

public interface IAuthService
{
    Task<string> Register(RegisterModel register);

    Task<LoginResponseDto> Login(LoginModel login);
}