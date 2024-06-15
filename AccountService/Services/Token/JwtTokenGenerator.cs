using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AccountService.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AccountService.Services.Token;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOption _jwtOption;
    private readonly ILogger<JwtTokenGenerator> _logger;

    public JwtTokenGenerator(IOptions<JwtOption> jwtOption, ILogger<JwtTokenGenerator> logger)
    {
        _logger = logger;
        _jwtOption = jwtOption.Value;
    }

    public string GeneratorToken(AppUser appUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        _logger.LogInformation($"key secry ={_jwtOption.Secret}");
        var key = Encoding.ASCII.GetBytes(_jwtOption.Secret);


        var claim = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, appUser.UserName),
            new Claim(JwtRegisteredClaimNames.Sub, appUser.Id),
            new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
            new Claim("scope", "orderService.fullAccess"),
            new Claim("scope", "orderService.Management"),
            new Claim("scope", "basketService.fullAccess"),
            new Claim("scope","productService.Management"),
        };

        var tokenDescription = new SecurityTokenDescriptor
        {
            Audience = _jwtOption.Audience,
            Issuer = _jwtOption.Issuer,
            Subject = new ClaimsIdentity(claim),
            Expires = DateTime.Now.AddMinutes(10),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescription);

        return tokenHandler.WriteToken(token);
    }
}