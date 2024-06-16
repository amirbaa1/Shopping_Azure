using AccountService.Model;
using AccountService.Model.Dto;
using AccountService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Controllers;

[ApiController]
[Route("Api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ResponseDto _responseDto;


    public AuthController(ILogger<AuthController> logger, IAuthService authService, UserManager<AppUser> userManager,
        ResponseDto responseDto)
    {
        _logger = logger;
        _authService = authService;
        _userManager = userManager;
        _responseDto = responseDto;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel register)
    {
        var user = await _authService.Register(register);
        if (string.IsNullOrEmpty(user))
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = user;
            return BadRequest(_responseDto);
        }

        _responseDto.Result = user;
        return Ok(_responseDto);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var user = await _authService.Login(loginModel);
        if (user.userDto == null)
        {
            _responseDto.IsSuccess = false;
            _responseDto.Message = "UserName and Password problem !!!";
            return BadRequest(_responseDto);
        }

        _responseDto.Result = user;
        return Ok(_responseDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Profile(string id)
    {
        var user = await _authService.ProfileService(id);
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateProfile updateProfile)
    {
        var user = await _authService.UpdateProfile(id, updateProfile);
        if (user.IsSuccess)
        {
            return Ok(user);
        }

        return BadRequest(user);
    }

    [HttpPost("activate/{userId}")]
    public async Task<IActionResult> ConfirmationEmail(string userId)
    {
        var userConfirm = await _authService.SendActivateEmail(userId);
        if (userConfirm == null)
        {
            return BadRequest(userConfirm);
        }

        return Ok(userConfirm);
    }

    [HttpPost("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmail confirmEmail)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == confirmEmail.userId);

        if (user == null)
        {
            return NotFound("NotFound User.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, confirmEmail.Token);
        // _logger.LogInformation($"Token :{confirmEmail.Token}");
        _logger.LogInformation($"Result : {result}");

        if (!result.Succeeded)
        {
            return BadRequest("No active Email.");
        }

        // تایید ایمیل با موفقیت انجام شده است
        return Ok($"Email confirmed successfully, {user}");
    }
}