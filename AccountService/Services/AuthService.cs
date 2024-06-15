using AccountService.Data;
using AccountService.Model;
using AccountService.Model.Dto;
using AccountService.Services.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly AccountDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(ILogger<AuthService> logger, AccountDbContext context, UserManager<AppUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string> Register(RegisterModel register)
    {
        var user = new AppUser
        {
            UserName = register.UserName,
            Email = register.Email,
            PhoneNumber = register.PhoneNumber,
            NormalizedEmail = register.Email.ToUpper(),
        };

        try
        {
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                // var userDto = new UserDto
                // {
                //     ID = user.Id,
                //     UserName = user.UserName,
                //     Email = user.Email,
                //     PhoneNumber = user.PhoneNumber,
                // };

                // var createToken = await  _userManager.GenerateEmailConfirmationTokenAsync(user);

                return "Create Register";
            }
            else
            {
                return result.Errors.FirstOrDefault().Description;
            }
        }
        catch (Exception e)
        {
            _logger.LogError($"error : {e.Message}");
            throw;
        }
    }

    public async Task<LoginResponseDto> Login(LoginModel login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == login.UserName.ToLower());
        bool isValid = await _userManager.CheckPasswordAsync(user, login.Password);
        if (isValid == false)
        {
            return new LoginResponseDto()
            {
                userDto = null,
                Token = ""
            };
        }
        else
        {
            var token = _jwtTokenGenerator.GeneratorToken(user);
            var userDto = new UserDto
            {
                ID = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
            };
            return new LoginResponseDto()
            {
                userDto = userDto,
                Token = token,
            };
        }
    }

    public async Task<ResponseDto> ProfileService(string Id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == Id);
        if (user == null)
        {
            return new ResponseDto

            {
                IsSuccess = false,
                Message = "NotFound!",
                Result = null
            };
        }

        var userDto = new ProfileDto
        {
            UserID = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            NumberPhone = user.PhoneNumber,
            Image = user.Image,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            EmailConfirmed = user.EmailConfirmed,
            AccessFailedCount = user.AccessFailedCount,
            LockoutEnabled = user.LockoutEnabled,
            TwoFactorEnabled = user.TwoFactorEnabled
        };
        return new ResponseDto
        {
            IsSuccess = true,
            Message = "ProfileService",
            Result = userDto
        };
    }

    public async Task<ResponseDto> UpdateProfile(string id, UpdateProfile updateProfiles)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
        {
            return new ResponseDto

            {
                IsSuccess = false,
                Message = "NotFound!",
                Result = null
            };
        }

        user.FirstName = updateProfiles.FirstName;
        user.LastName = updateProfiles.LastName;
        user.Image = updateProfiles.Image;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Profile updated successfully!",
                Result = user
            };
        }
        else
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Update failed!",
                Result = result.Errors
            };
        }
    }
}