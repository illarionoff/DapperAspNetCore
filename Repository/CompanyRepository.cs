using Dapper;
using DapperAspNetCore.Context;
using DapperAspNetCore.Contracts;
using DapperAspNetCore.Dto;
using DapperAspNetCore.Entities;
using System.Data;

namespace DapperAspNetCore.Repository;

public class CompanyRepository  :ICompanyRepository
{
    private readonly AppDbContext _context;

    public CompanyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Company> CreateCompany(CompanyForCreationDto company)
    {
        var query = "INSERT INTO COMPANIES (Name, Address, Country) VALUES (@Name, @Address, @Country)" + 
            "SELECT CAST(SCOPE_IDENTITY() AS int)";

        var parameters = new DynamicParameters();
        parameters.Add("Name", company.CompanyName, DbType.String);
        parameters.Add("Address", company.Address, DbType.String);
        parameters.Add("Country", company.Country, DbType.String);

        using (var connection = _context.CreateConnection())
        {
            var id = await connection.QuerySingleAsync<int>(query, parameters);

            var created = new Company
            {
                Id = id,
                CompanyName = company.CompanyName,
                Address = company.Address,
                Country = company.Country,
            };

            return created;
        }
    }

    public async Task<IEnumerable<Company>> GetCompaniesAsync()
    {
        var query = "SELECT Id, Name as CompanyName, Address, Country FROM COMPANIES";

        using (var connection = _context.CreateConnection())
        {
            var companies = await connection.QueryAsync<Company>(query);
            return companies.ToList();
        }
    }

    public async Task<Company> GetCompanyByIdAsync(int id)
    {
        var query = $"SELECT Id, Name as CompanyName, Address, Country FROM COMPANIES where Id = @Id";

        using (var connection = _context.CreateConnection())
        {
            var company = await connection.QuerySingleOrDefaultAsync<Company>(query, new { id });
            return company;
        }
    }
}
