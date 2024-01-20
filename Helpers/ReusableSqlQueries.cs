using System.Data;
using Dapper;
using DotnetApi.Data;
using DotnetApi.Models;

namespace DotnetApi.Helpers;

public class ReusableSqlQueries
{
    private DataContextDapper _dapper;
    public ReusableSqlQueries(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    public bool UserupSert(UserComplete user)
    {
        string sqlUpSert = " EXEC TutorialAppSchema.spUser_Upsert " +
                            "@FirstName = @firstNameParam," +
                            "@LastName = @lastNameParam," +
                            "@Email = @emailParam," +
                            "@Gender = @genderParam," +
                            "@JobTitle = @jobTitleParam," +
                            "@Department = @departmentParam," +
                            "@Salary = @salaryParam," +
                            "@Active = @activeParam," +
                            "@UserId = @userIdParam  ";

        var parameters = new DynamicParameters();
        parameters.Add("@firstNameParam", $"{user.FirstName}", DbType.String);
        parameters.Add("@lastNameParam", $"{user.LastName}", DbType.String);
        parameters.Add("@emailParam", $"{user.Email}", DbType.String);
        parameters.Add("@genderParam", $"{user.Gender}", DbType.String);
        parameters.Add("@jobTitleParam", $"{user.JobTitle}", DbType.String);
        parameters.Add("@departmentParam", $"{user.Department}", DbType.String);
        parameters.Add("@salaryParam", user.Salary, DbType.Decimal);
        parameters.Add("@activeParam", $"{user.Active}", DbType.Boolean);
        parameters.Add("@userIdParam", user.UserId, DbType.Int32);


        return _dapper.Execute(sqlUpSert, parameters) == 0;
    }
}