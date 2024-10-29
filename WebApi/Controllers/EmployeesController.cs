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
    /// Controller for managing employee information.
    /// Provides endpoints to perform CRUD operations on employees.
    /// </summary>
    [RoutePrefix("api/v1/employees")]
    public class EmployeesController : ApiController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly IValidator<EmployeeDto> _employeeDtoValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesController"/> class.
        /// </summary>
        /// <param name="employeeService">Service for handling employee data.</param>
        /// <param name="mapper">Mapper for converting between DTO and model.</param>
        /// <param name="employeeValidator">Validator for Employee DTO.</param>
        public EmployeesController(IEmployeeService employeeService, IMapper mapper, IValidator<EmployeeDto> employeeValidator)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _employeeDtoValidator = employeeValidator;
        }

        /// <summary>
        /// Retrieves all employees.
        /// </summary>
        /// <returns>A list of all employees.</returns>
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Employees()
        {
            var items = await _employeeService.GetAllEmployeesAsync();
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(items);
            return Ok(employeeDtos);
        }

        /// <summary>
        /// Retrieves an employee by their code.
        /// </summary>
        /// <param name="employeeCode">The code of the employee to retrieve.</param>
        /// <returns>The employee matching the specified code.</returns>
        [HttpGet, Route("{employeeCode}")]
        public async Task<IHttpActionResult> Employees(string employeeCode)
        {
            if (string.IsNullOrEmpty(employeeCode))
            {
                return BadRequest("employeeCode is required");
            }
            var item = await _employeeService.GetEmployeeByCodeAsync(employeeCode);
            if (item == null)
            {
                return NotFound();
            }
            var employeeDto = _mapper.Map<EmployeeDto>(item);
            return Ok(employeeDto);
        }

        /// <summary>
        /// Creates a new employee.
        /// </summary>
        /// <param name="employeeDto">The data of the employee to create.</param>
        /// <returns>The result of the creation process, including the created employee if successful.</returns>
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Employees([FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
                return BadRequest("Employee data is null");

            // Validate the DTO with FluentValidation
            var validationResult = await _employeeDtoValidator.ValidateAsync(employeeDto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return BadRequest(ModelState);
            }

            var employeeInfo = _mapper.Map<EmployeeInfo>(employeeDto);
            var result = await _employeeService.CreateEmployeeAsync(employeeInfo);

            if (!result.IsSuccess)
            {
                return Content(HttpStatusCode.Conflict, result.Message);
            }
            var createdEmployeeUri = Url.Link("DefaultApi", new { id = employeeDto.EmployeeCode });
            return Content(HttpStatusCode.Created, new { employee = employeeDto, location = createdEmployeeUri });
        }

        /// <summary>
        /// Updates an existing employee by their code.
        /// </summary>
        /// <param name="employeeCode">The code of the employee to update.</param>
        /// <param name="employeeDto">The new data for the employee.</param>
        /// <returns>The result of the update process, including the updated employee if successful.</returns>
        [HttpPut, Route("{employeeCode}")]
        public async Task<IHttpActionResult> Employees(string employeeCode, [FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Request is null");
            }

            // Validate the DTO with FluentValidation
            var validationResult = await _employeeDtoValidator.ValidateAsync(employeeDto);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            var employeeInfo = _mapper.Map<EmployeeInfo>(employeeDto);
            var result = await _employeeService.UpdateEmployeeByCodeAsync(employeeCode, employeeInfo);

            if (!result.IsSuccess)
                return Content(HttpStatusCode.NotFound, result.Message);

            return Ok(new { Message = result.Message });
        }

        /// <summary>
        /// Deletes an employee by their code.
        /// </summary>
        /// <param name="employeeCode">The code of the employee to delete.</param>
        /// <returns>The result of the deletion process, including a success message if the deletion was successful.</returns>
        [HttpDelete, Route("{employeeCode}")]
        public async Task<IHttpActionResult> Employee(string employeeCode)
        {
            var result = await _employeeService.DeleteEmployeeByCodeAsync(employeeCode);
            if (!result.IsSuccess)
                return Content(HttpStatusCode.NotFound, result.Message);

            return Ok(new { result.Message });
        }
    }
}
