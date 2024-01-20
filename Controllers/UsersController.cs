using Microsoft.AspNetCore.Mvc;
using DotnetApi.Models;
using DotnetApi.Data;
using Dapper;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using DotnetApi.Helpers;

namespace DotnetApi.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private DataContextDapper _dapper;
    private ReusableSqlQueries _reusableSqlQueries;
    // constructor
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSqlQueries = new ReusableSqlQueries(config);

    }
    // http methods
    [HttpGet("allUsers/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetAllUsers(int userId, bool isActive)
    {
        DynamicParameters parameters = new();
        string sqlQuery = "EXEC TutorialAppSchema.spUsers_Get ";
        string param = "";

        if (userId > 0)
        {
            param += $", @UserId = @UserIdParam";
            parameters.Add("@UserIdParam", userId, DbType.Int32, ParameterDirection.Input);
        }

        if (isActive == true)
        {
            param += $", @Active = @ActiveParam";
            parameters.Add("@ActiveParam", isActive, DbType.Boolean, ParameterDirection.Input);
        }

        if (param.Contains(','))
        {
            sqlQuery += param.Substring(1);
        }

        return _dapper.LoadData<UserComplete>(sqlQuery, parameters).ToList();
    }

    [HttpPost("UserupSert")]
    public ActionResult UserupSert(UserComplete user)
    {
        return _reusableSqlQueries.UserupSert(user) ? BadRequest() : Ok();
    }


    [HttpDelete("DeletUsers/{userId}")]
    public ActionResult DeletUsers(int userId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@userIdParam", userId, DbType.Int32, ParameterDirection.Input);

        var userDelete = "EXEC TutorialAppSchema.spUser_Delete @UserId = @userIdParam";

        if (_dapper.Execute(userDelete, parameters) == 0) return BadRequest();

        return NoContent();
    }

    [HttpGet("PostsSpGet/{userId}/{postId}/{searchValue}")]
    public ActionResult<Post> PostsSpGet(int userId, int postId, string searchValue = "None")
    {
        var parameters = new DynamicParameters();

        string sqlPostUpSert = "EXEC TutorialAppSchema.spPosts_Get ";
        var greenList = "";

        if (userId != 0)
        {
            greenList += ", @UserId = @userIdParam ";
            parameters.Add("@userIdParam ", userId, DbType.Int32, ParameterDirection.Input);
        }
        if (postId != 0)
        {
            greenList += ", @PostId = @postIdParam ";
            parameters.Add("@postIdParam", postId, DbType.Int32, ParameterDirection.Input);
        }
        if (searchValue.ToLower() != "none")
        {
            greenList += ", @SearchValue = @searchValueParam";
            parameters.Add("@searchValueParam", searchValue, DbType.String, ParameterDirection.Input);
        }

        if (greenList.Length > 0)
        {
            sqlPostUpSert += greenList.Substring(1);
        }

        var response = _dapper.LoadData<Post>(sqlPostUpSert, parameters).ToList();

        if (response.Count == 0) return NotFound();
        if (response.Count > 0) return Ok(response);

        return BadRequest();
    }

    [HttpGet("MyPosts")]
    public ActionResult<Post> MyPosts()
    {
        var parameters = new DynamicParameters();
        string sqlPostUpSert = "EXEC TutorialAppSchema.spPosts_Get  @UserId =  " + User.FindFirst("userId")?.Value ?? "";

        var response = _dapper.LoadData<Post>(sqlPostUpSert, parameters).ToList();

        if (response.Count == 0) return NotFound();
        if (response.Count > 0) return Ok(response);

        return BadRequest();
    }

    [HttpPut("UpSertPosts")]
    public ActionResult UpSertPosts(Post post)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@postTitleParam ", post.PostTitle, DbType.String, ParameterDirection.Input);
        parameters.Add("@postContentParam ", post.PostContent, DbType.String, ParameterDirection.Input);

        string sqlPostUpSert = " EXEC TutorialAppSchema.spPosts_Upsert " +
                                    " @PostTitle = @postTitleParam " +
                                    ", @PostContent = @postContentParam " +
                                    ", @UserId = " + User.FindFirst("userId")?.Value ?? "";


        if (post.PostId > 0)
        {
            parameters.Add("@postIdParam", post.PostId, DbType.Int32, ParameterDirection.Input);
            sqlPostUpSert += ", @PostId = @postIdParam ";
        }


        if (_dapper.Execute(sqlPostUpSert, parameters) == 0) return BadRequest();

        return Ok();
    }

    [HttpDelete("DeleteSpPost/{postId}")]
    public ActionResult DeleteSpPost(int postId)
    {
        var parameters = new DynamicParameters();
        string sqlDeleteSpPost = "EXEC TutorialAppSchema.spPost_Delete " +
                                    "@UserId = " + User.FindFirst("userId")?.Value ?? "";

        if (postId > 0)
        {
            parameters.Add("@postIdParam", postId, DbType.Int32, ParameterDirection.Input);
            sqlDeleteSpPost += ", @PostId = @postIdParam ";
        }

        if (_dapper.Execute(sqlDeleteSpPost, parameters) == 0) return BadRequest();

        return NoContent();

    }
}