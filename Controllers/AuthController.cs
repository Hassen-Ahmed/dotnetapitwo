using System.Security.Cryptography;
using AutoMapper;
using DotnetApi.Data;
using DotnetApi.Dto;
using DotnetApi.Helpers;
using DotnetApi.Models;
using DotnetApi.Models.Auth;
using DotnetApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private ReusableSqlQueries _reusableSqlQueries;
    private DataContextEF _context;
    private IMapper _mapper;

    private IAuthControllerService _authControllerService;
    public AuthController(DataContextEF context, IAuthControllerService authControllerService, IConfiguration config)
    {
        _context = context;
        _authControllerService = authControllerService;
        _reusableSqlQueries = new ReusableSqlQueries(config);
        _mapper = new Mapper(new MapperConfiguration(
            cfg => cfg.CreateMap<UserRegistration, UserComplete>()
        ));
    }

    [Authorize]
    [HttpPost("ResetPassword")]

    public async Task<IActionResult> ResetPassword(UserResetPasswordDto userForResetPassword)
    {
        if (userForResetPassword.Password != userForResetPassword.PasswordConfirm) throw new Exception("Password does not match!");
        Auth? response = await _context.Auth.Where(u => u.Email == userForResetPassword.Email).FirstOrDefaultAsync();

        byte[] passwordSalt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(passwordSalt);
        }

        byte[] passwordHash = _authControllerService.GeneratePasswordHash($"{userForResetPassword.Password}", passwordSalt);

        if (response is not null)
        {
            response.PasswordHash = passwordHash;
            response.PasswordSalt = passwordSalt;

            return await _context.SaveChangesAsync() > 0 ? Ok() : StatusCode(401, "Unable to save the user credential.");
        }

        else
        {
            throw new Exception("Something went Wrong!");
        }

    }

    [AllowAnonymous]
    [HttpPost("Registarion")]
    public async Task<IActionResult> Register(UserRegistration userRegistration)
    {
        if (userRegistration.Password != userRegistration.PasswordConfirm) throw new Exception("Password does not match!");

        Auth? response = await _context.Auth.Where(u => u.Email == userRegistration.Email).SingleOrDefaultAsync();
        if (response is not null)
            throw new Exception("Email already exist.");

        byte[] passwordSalt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(passwordSalt);
        }

        byte[] passwordHash = _authControllerService.GeneratePasswordHash($"{userRegistration.Password}", passwordSalt);

        Auth authEntity = new()
        {
            Email = userRegistration.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };
        await _context.Auth.AddAsync(authEntity);

        if (_context.SaveChanges() > 0)
        {
            UserComplete userComplete = _mapper.Map<UserComplete>(userRegistration);
            return _reusableSqlQueries.UserupSert(userComplete) ? StatusCode(401) : Ok();
        }

        else
            throw new Exception("Something wrong with Auth entity!");
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserLogin userLogin)
    {
        // check if exist
        Auth? responseAuthObj = await _context.Auth.Where(a => a.Email == userLogin.Email).SingleOrDefaultAsync();

        if (responseAuthObj is null) return StatusCode(401, "Wrong Email address.");

        // check if password is correct by create hash and salt. 
        byte[] passwordHash = _authControllerService.GeneratePasswordHash($"{userLogin.Password}", responseAuthObj.PasswordSalt);

        for (int i = 0; i < passwordHash.Length; i++)
            if (passwordHash[i] != responseAuthObj.PasswordHash[i]) return StatusCode(401, "Unauthorized Request!");

        int userId = await _context.Users.Where(u => u.Email == userLogin.Email).Select(u => u.UserId).SingleOrDefaultAsync();

        return StatusCode(201, new Dictionary<string, string> {
            {"token" , _authControllerService.GenerateToken(userId)}
        });
    }

    [HttpGet("RefreshToken")]
    public async Task<string> RefreshToken()
    {
        bool convertToInt = int.TryParse(User.FindFirst("userId")?.Value ?? "", out int paresedInt);

        int userId = await _context.Users
                        .Where(u => u.UserId == paresedInt)
                        .Select(u => u.UserId)
                        .SingleOrDefaultAsync();

        return _authControllerService.GenerateToken(userId);
    }

}