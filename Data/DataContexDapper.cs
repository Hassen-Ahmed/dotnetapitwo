using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetApi.Data
{
    public class DataContextDapper
    {
        // field
        private readonly IDbConnection _dbConnection;
        // constructor
        public DataContextDapper(IConfiguration config)
        {
            _dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }
        // methods
        public IEnumerable<T> LoadData<T>(string sql, DynamicParameters parameters)
        {
            return _dbConnection.Query<T>(sql, parameters);
        }
        public T LoadDataSingle<T>(string sql)
        {
            return _dbConnection.QuerySingle(sql);
        }
        public int Execute(string sql, DynamicParameters parameters)
        {
            return _dbConnection.Execute(sql, parameters);
        }
    }
}