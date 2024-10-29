using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using FluentValidation;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller for managing company information.
    /// Provides endpoints to perform CRUD operations on companies.
    /// </summary>
    [RoutePrefix("api/v1/companies")]
    public class CompaniesController : ApiController
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly IValidator<CompanyDto> _companyDtoValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompaniesController"/> class.
        /// </summary>
        /// <param name="companyService">Service for handling company data.</param>
        /// <param name="mapper">Mapper for converting between DTO and model.</param>
        /// <param name="companyValidator">Validator for Company DTO.</param>
        public CompaniesController(ICompanyService companyService, IMapper mapper, IValidator<CompanyDto> companyValidator)
        {
            _companyService = companyService;
            _mapper = mapper;
            _companyDtoValidator = companyValidator;
        }

        /// <summary>
        /// Retrieves all companies.
        /// </summary>
        /// <returns>A list of all companies.</returns>
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Companies()
        {
            var items = await _companyService.GetAllCompaniesAsync();
            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(items);
            return Ok(companyDtos);
        }

        /// <summary>
        /// Retrieves a company by its code.
        /// </summary>
        /// <param name="companyCode">The code of the company to retrieve.</param>
        /// <returns>The company matching the specified code.</returns>
        [HttpGet, Route("{companyCode}")]
        public async Task<IHttpActionResult> Companies(string companyCode)
        {
            if(string.IsNullOrEmpty(companyCode ))
            {
                return BadRequest("companyCode is required");
            }
            var item = await _companyService.GetCompanyByCodeAsync(companyCode);
            if (item == null)
            {
                return NotFound();
            }
            var companyDto = _mapper.Map<CompanyDto>(item);
            return Ok(companyDto);
        }

        /// <summary>
        /// Creates a new company.
        /// </summary>
        /// <param name="companyDto">The data of the company to create.</param>
        /// <returns>The result of the creation process, including the created company if successful.</returns>
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Companies([FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
                return BadRequest("Company data is null");

            // Validate the DTO with FluentValidation
            var validationResult = await _companyDtoValidator.ValidateAsync(companyDto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return BadRequest(ModelState);
            }

            var companyInfo = _mapper.Map<CompanyInfo>(companyDto);
            var result = await _companyService.CreateCompanyAsync(companyInfo);

            if (!result.IsSuccess)
            {
                return Content(HttpStatusCode.Conflict, result.Message);
            }
            var createdCompanyUri = Url.Link("DefaultApi", new { id = companyDto.CompanyCode });
            return Content(HttpStatusCode.Created, new { company = companyDto, location = createdCompanyUri });
        }

        /// <summary>
        /// Updates an existing company by its code.
        /// </summary>
        /// <param name="companyCode">The code of the company to update.</param>
        /// <param name="companyDto">The new data for the company.</param>
        /// <returns>The result of the update process, including the updated company if successful.</returns>
        [HttpPut, Route("{companyCode}")]
        public async Task<IHttpActionResult> Companies(string companyCode, [FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return BadRequest("Request is null");
            }

            // Validate the DTO with FluentValidation
            var validationResult = await _companyDtoValidator.ValidateAsync(companyDto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            var companyInfo = _mapper.Map<CompanyInfo>(companyDto);
            var result = await _companyService.UpdateCompanyByCodeAsync(companyCode, companyInfo);

            if (!result.IsSuccess)
                return Content(HttpStatusCode.NotFound, result.Message);

            return Ok(new { Message = result.Message });
        }

        /// <summary>
        /// Deletes a company by its code.
        /// </summary>
        /// <param name="companyCode">The code of the company to delete.</param>
        /// <returns>The result of the deletion process, including a success message if the deletion was successful.</returns>
        [HttpDelete, Route("{companyCode}")]
        public async Task<IHttpActionResult> Company(string companyCode)
        {
            var result = await _companyService.DeleteCompanyByCodeAsync(companyCode);
            if (!result.IsSuccess)
                return Content(HttpStatusCode.NotFound, result.Message);

            return Ok(new { result.Message });
        }
    }

}

