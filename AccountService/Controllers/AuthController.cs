using AccountService.Model;
using AccountService.Model.Dto;
using AccountService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
}