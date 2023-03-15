using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperAspNetCore.Context;

public class AppDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public AppDbContext(IConfiguration configuration)
	{
        _configuration = configuration;
        _connectionString = configuration.GetConnectionString("Default");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
