namespace AccountService.Model;

public class ConfirmEmail
{
    public string userId { get; set; }

    public string Token { get; set; }
}