using DapperAspNetCore.Dto;
using DapperAspNetCore.Entities;

namespace DapperAspNetCore.Contracts;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetCompaniesAsync();
    Task<Company> GetCompanyByIdAsync(int id);
    Task<Company> CreateCompany(CompanyForCreationDto company);
}
