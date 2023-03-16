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

    public async Task<Company> CreateCompanyAsync(CompanyForCreationDto company)
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

    public async Task UpdateCompanyAsync(int id, CompanyForUpdateDto company)
    {
        var query = $"UPDATE COMPANIES SET Name = @Name, Address = @Address, Country = @Country  WHERE Id = @Id";
        
        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int32);
        parameters.Add("Name", company.CompanyName, DbType.String);
        parameters.Add("Address", company.Address, DbType.String);
        parameters.Add("Country", company.Country, DbType.String);
 
        using (var connection = _context.CreateConnection())
        {
             await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task DeleteCompanyAsync(int id)
    {
        var query = $"DELETE FROM COMPANIES WHERE Id = @Id";

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, new { id });
        }
    }

    public async Task<Company> GetCompanyByEmployeeIdAsync(int id)
    {
        var procedureName = "ShowCompanyByEmployeeId";
        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

        using (var connection = _context.CreateConnection())
        {
            var company  = await connection.QueryFirstOrDefaultAsync<Company>(procedureName,parameters, commandType: CommandType.StoredProcedure);
            return company;
        }
    }

    public async Task<Company?> GetMultipleResults(int id)
    {
        var query = $"SELECT Id, Name as CompanyName, Address, Country FROM COMPANIES where Id = @Id;" +
            $"SELECT* FROM EMPLOYEES where CompanyId = @Id;";

        using (var connection = _context.CreateConnection())
        using (var multi = await connection.QueryMultipleAsync(query, new { id }))
        {
            var company = await multi.ReadSingleOrDefaultAsync<Company>();
            if(company is not null)
            {
                company.Employees = (await multi.ReadAsync<Employee>()).ToList();
            }

            return company;
        }

    }

    public async Task<List<Company>> GetMultipleMapping()
    {
        var query = $"SELECT * FROM COMPANIES c JOIN EMPLOYEES e ON c.Id = e.CompanyId";

        using (var connection = _context.CreateConnection())
        {
            var companyDict = new Dictionary<int, Company>();
            var companies = await connection.QueryAsync<Company, Employee, Company>(
                query, (company, employee) =>
                {
                    if(!companyDict.TryGetValue(company.Id, out var currentCompany))
                    {
                        currentCompany = company;
                        companyDict.Add(currentCompany.Id, currentCompany);
                    }

                    currentCompany.Employees.Add(employee);

                    return currentCompany;
                }
            );

            return companies.Distinct().ToList();
        }
    }

    public async Task CreateMultipleCompanies(List<CompanyForCreationDto> companies)
    {
        var query = "INSERT INTO COMPANIES (Name, Address, Country) VALUES (@Name, @Address, @Country)";

        using (var connection = _context.CreateConnection())
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var company in companies)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Name", company.CompanyName, DbType.String);
                    parameters.Add("Address", company.Address, DbType.String);
                    parameters.Add("Country", company.Country, DbType.String);

                    await connection.ExecuteAsync(query, parameters, transaction: transaction);
                }

                transaction.Commit();
            }
        };
    }
}
