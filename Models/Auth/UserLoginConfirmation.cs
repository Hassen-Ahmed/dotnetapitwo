namespace DotnetApi.Models.Auth;

public class UserLoginConfirmation
{
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public UserLoginConfirmation()
    {
        PasswordHash ??= [];
        PasswordSalt ??= [];
    }
}