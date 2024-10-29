using DataAccessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Model.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByCodeAsync(string employeeCode);
        Task<DbResultInfo> CreateEmployeeAsync(Employee employee);
        Task<DbResultInfo> UpdateByCodeAsync(string employeeCode, Employee employee);
        Task<bool> DeleteByCodeAsync(string employeeCode);
        Task<Employee> GetEmployeeByNameAsync(string employeeName);
    }
}
