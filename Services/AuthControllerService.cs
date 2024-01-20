using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.Services;

public class AuthControllerService : IAuthControllerService
{
    private readonly IConfiguration _config;
    public AuthControllerService(IConfiguration config)
    {
        _config = config;
    }

    public byte[] GeneratePasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusPasswordKey = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

        byte[] passwordHash = KeyDerivation.Pbkdf2(
        password: $"{password}",
        salt: Encoding.UTF8.GetBytes(passwordSaltPlusPasswordKey),
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 100000,
        numBytesRequested: 256 / 8);

        return passwordHash;
    }

    public string GenerateToken(int userId)
    {
        Claim[] claims = [
            new Claim("userId", userId.ToString()),
            new Claim("role", "User")
        ];

        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

        SymmetricSecurityKey tokenKey = new(
            Encoding.UTF8.GetBytes($"{tokenKeyString}")
        );

        SigningCredentials credentials = new(
            tokenKey,
            SecurityAlgorithms.HmacSha512Signature
        );

        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1)
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);

    }
}