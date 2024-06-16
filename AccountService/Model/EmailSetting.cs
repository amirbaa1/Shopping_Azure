namespace AccountService.Model;

public class EmailSetting
{
    //stmp email
    public string HOST { get; set; } = "smtp.elasticemail.com";
    public int PORT { get; set; } = 2525;
    public string User { get; set; } = "amir.2002.ba@gmail.com";
    public string Password { get; set; } = "69985A8E31CB9CF00B0284B8A37C1EDEE8D8";

    // sandGrid
    // public string ApiKey { get; set; } = "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";
    // public string FromAddress { get; set; } = "amir.2002.ba@gmail.com";
    // public string FromName { get; set; } = "shooping";
}