namespace AccountService.Services.Token;

public class JwtOption
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? Secret { get; set; }
}