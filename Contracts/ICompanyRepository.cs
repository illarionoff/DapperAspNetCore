using DapperAspNetCore.Dto;
using DapperAspNetCore.Entities;

namespace DapperAspNetCore.Contracts;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetCompaniesAsync();
    Task<Company> GetCompanyByIdAsync(int id);
    Task<Company> CreateCompanyAsync(CompanyForCreationDto company);
    Task UpdateCompanyAsync(int id, CompanyForUpdateDto company);
    Task DeleteCompanyAsync(int id);
    Task<Company> GetCompanyByEmployeeIdAsync(int id);
    Task<Company?> GetMultipleResults(int id);
    Task<List<Company>> GetMultipleMapping();
}
