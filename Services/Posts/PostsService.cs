using DotnetApi.Data;
using DotnetApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Services.Posts;

public class PostsService : IPostsService
{
    private DataContextEF _context;
    public PostsService(DataContextEF context)
    {
        _context = context;
    }
    public async Task<List<Post>> GetPostsService()
    {
        List<Post> response = await _context.Posts.AsNoTracking().ToListAsync();
        return response;
    }
}