
using System.Data;
using DotnetApi.Data;
using DotnetApi.Dto.POST;
using DotnetApi.Dto.PUT;
using DotnetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private DataContextEF _context;

    public PostController(DataContextEF context)
    {
        _context = context;
    }

    [HttpGet("posts")]
    public async Task<ActionResult<List<Post>>> GetPosts()
    {
        List<Post> response = await _context.Posts.AsNoTracking().ToListAsync();

        return Ok(response ?? throw new Exception("Unable to find Posts."));
    }

    [HttpGet("postSingle/{postId}")]
    public async Task<ActionResult<Post>> GetPosts(int postId)
    {
        Post? response = await _context.Posts.Where(p => p.PostId == postId).AsNoTracking().FirstOrDefaultAsync();

        return response is null ? throw new Exception("Wrong PostId") : Ok(response);
    }

    // [AllowAnonymous]
    [HttpGet("myposts")]
    public async Task<ActionResult<List<Post>>> GetMyPosts()
    {
        int userId = int.Parse($"{User.FindFirst("userId")?.Value}");
        List<Post> response = await _context.Posts.Where(p => p.UserId == userId).AsNoTracking().ToListAsync();

        return response is null ? throw new Exception("Unable to find Posts") : Ok(response);

    }

    [HttpGet("PostBySearch/{searchParam}")]
    public async Task<ActionResult<List<Post>>> PostBySearch(string searchParam)
    {
        List<Post> response = await _context.Posts
                                .Where(p =>
                                    EF.Functions.Like(p.PostTitle, $"%{searchParam}%")
                                        ||
                                    EF.Functions.Like(p.PostContent, $"%{searchParam}%"))
                                .AsNoTracking()
                                .ToListAsync();

        return response.Count == 0 ? NotFound() : Ok(response);
    }

    [HttpPost("post")]
    public async Task<IActionResult> PostPost(PostPostDto postPostDto)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value ?? "");

        Post newPost = new Post()
        {
            UserId = userId,
            PostTitle = postPostDto.PostTitle,
            PostContent = postPostDto.PostContent,
            PostCreated = DateTime.Now,
            PostUpdated = DateTime.Now
        };

        await _context.Posts.AddAsync(newPost);

        if (await _context.SaveChangesAsync() > 0)
            return Accepted();

        return BadRequest();
    }


    [HttpPut("post")]
    public async Task<IActionResult> PostUpdate(PostPutDto postPutDto)
    {
        int userId = int.Parse(User.FindFirst("userId")?.Value ?? "");
        Post? selectedPost = await _context.Posts
                                .Where(p => p.PostId == postPutDto.PostId && p.UserId == userId)
                                .FirstOrDefaultAsync();

        if (selectedPost is null) return BadRequest();

        selectedPost.PostTitle = postPutDto.PostTitle;
        selectedPost.PostContent = postPutDto.PostContent;

        if (await _context.SaveChangesAsync() > 0)
            return Accepted();

        return BadRequest();
    }

    [HttpDelete("post/{postId}")]
    public async Task<IActionResult> PostDelete(int postId)
    {

        int userId = int.Parse(User.FindFirst("userId")?.Value ?? "");
        Post? selectedPost = await _context.Posts
                                .Where(p => p.PostId == postId && p.UserId == userId)
                                .FirstOrDefaultAsync();

        if (selectedPost is null) return BadRequest();

        _context.Posts.Remove(selectedPost);

        if (await _context.SaveChangesAsync() > 0)
            return Accepted();

        return BadRequest();
    }

}
