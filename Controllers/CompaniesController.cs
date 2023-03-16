using DapperAspNetCore.Contracts;
using DapperAspNetCore.Dto;
using DapperAspNetCore.Entities;
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
            var createdCompany = await _companyRepository.CreateCompanyAsync(company);

            return CreatedAtRoute("GetCompany", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyForUpdateDto company)
        {
            var dbCompany = _companyRepository.GetCompanyByIdAsync(id);

            if(dbCompany != null)
            {
                await _companyRepository.UpdateCompanyAsync(id, company);

                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var dbCompany = _companyRepository.GetCompanyByIdAsync(id);

            if (dbCompany != null)
            {
                await _companyRepository.DeleteCompanyAsync(id);

                return NoContent();
            }

            return NotFound();
        }

        [HttpGet("ByEmployee/{id}")]
        public async Task<IActionResult> GetCompanyForEmployee(int id)
        {
            var company = await _companyRepository.GetCompanyByEmployeeIdAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetMultipleResults(int id)
        {
            var company = await _companyRepository.GetMultipleResults(id);

            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        [HttpGet("MultipleMapping")]
        public async Task<IActionResult> GetMultipleMapping()
        {
            var companies = await _companyRepository.GetMultipleMapping();

            return Ok(companies);
        }
    }
}
