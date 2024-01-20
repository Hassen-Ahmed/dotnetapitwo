namespace DotnetApi.Services;

public interface IAuthControllerService
{
    byte[] GeneratePasswordHash(string password, byte[] passwordSalt);
    string GenerateToken(int userId);
}