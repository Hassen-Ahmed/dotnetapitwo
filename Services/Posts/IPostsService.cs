using DotnetApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Services.Posts;

public interface IPostsService
{
    Task<List<Post>> GetPostsService();
}