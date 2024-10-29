using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployeeInfo>> GetAllEmployeesAsync()
        {
            return await ExecuteAsync(async () =>
            {
                var companies = await _employeeRepository.GetAllEmployeesAsync();
                return _mapper.Map<IEnumerable<EmployeeInfo>>(companies);
            });
        }
        public async Task<EmployeeInfo> GetEmployeeByCodeAsync(string employeeCode)
        {
            return await ExecuteAsync(async () =>
            {
                var Employee = await _employeeRepository.GetEmployeeByCodeAsync(employeeCode);
                if (Employee == null)
                {
                    _logger.Warn($"Employee with employee code {employeeCode} not found.");
                    return null;
                }
                return _mapper.Map<EmployeeInfo>(Employee);
            });
        }

        public async Task<ResultInfo> CreateEmployeeAsync(EmployeeInfo EmployeeInfo)
        {
            return await ExecuteAsync(async () =>
            {
                var Employee = _mapper.Map<Employee>(EmployeeInfo);
                var saveResult = await _employeeRepository.CreateEmployeeAsync(Employee);
                return new ResultInfo { IsSuccess = saveResult.IsSuccess, Message = saveResult.Message };
            });
        }

        public async Task<ResultInfo> UpdateEmployeeByCodeAsync(string employeeCode, EmployeeInfo EmployeeInfo)
        {
            return await ExecuteAsync(async () =>
            {
                var Employee = _mapper.Map<Employee>(EmployeeInfo);
                var updateResult = await _employeeRepository.UpdateByCodeAsync(employeeCode, Employee);
                if (!updateResult.IsSuccess)
                {
                    _logger.Warn($"Employee with code {employeeCode} not found.");
                    return new ResultInfo { IsSuccess = false, Message = "Failed to update Employee details." };
                }
                return new ResultInfo { IsSuccess = true, Message = "Employee details updated successfully." };
            });
        }

        private async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                _logger.Error($"EmployeeService: An error occurred while processing the request - {ex.Message} :: {ex.InnerException.Message} ");
                return default;
            }
        }

        public async Task<ResultInfo> DeleteEmployeeByCodeAsync(string employeeCode)
        {

            var result = await _employeeRepository.DeleteByCodeAsync(employeeCode);
            if (!result)
            {
                _logger.Warn($"Employee with code {employeeCode} not found.");
                return new ResultInfo { IsSuccess = false, Message = "Employee not found." };
            }

            return new ResultInfo { IsSuccess = true, Message = $"Employee with code {employeeCode} was successfully deleted." };
        }
    }
}
