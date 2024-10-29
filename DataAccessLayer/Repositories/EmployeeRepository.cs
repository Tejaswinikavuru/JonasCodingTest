using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using NLog;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbWrapper<Employee> _employeeDbWrapper;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly AsyncRetryPolicy _retryPolicy;

        public EmployeeRepository(IDbWrapper<Employee> employeeDbWrapper)
        {
            _employeeDbWrapper = employeeDbWrapper ?? throw new ArgumentNullException(nameof(employeeDbWrapper));

            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.Warn($"Retry {retryCount} due to {exception.Message}. Waiting {timeSpan} before next retry.");
                    });
        }

        private async Task<T> ExecuteDbCallWithRetryAsync<T>(Func<Task<T>> operation, string errorMessage)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, errorMessage);
                    throw;
                }
            });
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await ExecuteDbCallWithRetryAsync(
                () => _employeeDbWrapper.FindAllAsync(),
                "Exception occurred while fetching all employees."
            );
        }

        public async Task<Employee> GetEmployeeByCodeAsync(string employeeCode)
        {
            return await ExecuteDbCallWithRetryAsync(
                async () =>
                {
                    var employees = await _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode));
                    return employees?.FirstOrDefault();
                },
                $"Exception occurred while fetching employee using employeeCode: {employeeCode}"
            );
        }

        public Task<DbResultInfo> CreateEmployeeAsync(Employee employee)
        {
            return ExecuteDbCallWithRetryAsync(async () =>
            {
                var existingEmployee = (await _employeeDbWrapper.FindAsync(t => t.SiteId.Equals(employee.SiteId) && t.EmployeeCode.Equals(employee.EmployeeCode)))?.FirstOrDefault();
                if (existingEmployee != null)
                {
                    return new DbResultInfo { IsSuccess = false, Message = "Employee already exists with the same EmployeeCode." };
                }

                var insertResult = await _employeeDbWrapper.InsertAsync(employee);
                return new DbResultInfo { IsSuccess = insertResult, Message = insertResult ? "Employee details saved successfully." : "Failed to save employee details." };
            }, $"Exception occurred while saving employee details: {employee}");
        }

        public Task<DbResultInfo> UpdateByCodeAsync(string employeeCode, Employee employee)
        {
            return ExecuteDbCallWithRetryAsync(async () =>
            {
                var existingEmployee = (await _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)))?.FirstOrDefault();
                if (existingEmployee == null)
                {
                    return new DbResultInfo { IsSuccess = false, Message = "Employee not found with the given employee code." };
                }

                if (Helper.AreEntitiesEqual(existingEmployee, employee))
                {
                    return new DbResultInfo { IsSuccess = true, Message = "Same employee details already exist." };
                }

                Helper.UpdateEntity(existingEmployee, employee);
                var updateResult = await _employeeDbWrapper.UpdateAsync(existingEmployee);

                return new DbResultInfo { IsSuccess = updateResult, Message = updateResult ? "Employee details updated successfully." : "Failed to update employee details." };
            }, $"Exception occurred while updating employee using employee code: {employeeCode}");
        }

        public Task<bool> DeleteByCodeAsync(string employeeCode)
        {
            return ExecuteDbCallWithRetryAsync(async () =>
            {
                var existingEmployee = (await _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)))?.FirstOrDefault();
                if (existingEmployee == null)
                {
                    _logger.Warn($"Employee with employee code {employeeCode} not available.");
                    return false;
                }

                var deleteResult = await _employeeDbWrapper.DeleteAsync(t => t.EmployeeCode.Equals(employeeCode));
                return deleteResult;
            }, $"Exception occurred while deleting employee using employee code: {employeeCode}");
        }

        public Task<Employee> GetEmployeeByNameAsync(string employeeName)
        {
            return ExecuteDbCallWithRetryAsync(
                async () =>
                {
                    var employees = await _employeeDbWrapper.FindAsync(t => t.EmployeeName.Equals(employeeName));
                    return employees?.FirstOrDefault();
                },
                $"Exception occurred while fetching employee by employee name: {employeeName}"
            );
        }
    }
}
