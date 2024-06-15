using AccountService.Model;
using AccountService.Model.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Services;

public interface IAuthService
{
    Task<string> Register(RegisterModel register);

    Task<LoginResponseDto> Login(LoginModel login);

    Task<ResponseDto> ProfileService(string Id);

    Task<ResponseDto> UpdateProfile(string id,UpdateProfile updateProfiles);
}