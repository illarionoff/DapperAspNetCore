using DapperAspNetCore.Contracts;
using DapperAspNetCore.Dto;
using Microsoft.AspNetCore.Mvc;

namespace DapperAspNetCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;

        public CompaniesController(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _companyRepository.GetCompaniesAsync();
            return Ok(companies);
        }

        [HttpGet("{id}", Name = "GetCompany")]
        public async Task<IActionResult> GetCompany(int id)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(id);

            if(company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            var createdCompany = await _companyRepository.CreateCompany(company);

            return CreatedAtRoute("GetCompany", new { id = createdCompany.Id }, createdCompany);
        }
    }
}
