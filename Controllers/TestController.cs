
using System.Data;
using DotnetApi.Data;
using DotnetApi.Dto.POST;
using DotnetApi.Dto.PUT;
using DotnetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult GetPosts()
    {
        return Ok("Hi, there.");
    }
    [HttpGet("/")]
    public IActionResult Gettest()
    {
        return Ok("Hi, there two.");
    }
}