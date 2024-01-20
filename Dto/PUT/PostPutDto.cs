namespace DotnetApi.Dto.PUT;

public partial class PostPutDto
{
    public int PostId { get; set; }
    public string? PostTitle { get; set; } = "";
    public string? PostContent { get; set; } = "";

}