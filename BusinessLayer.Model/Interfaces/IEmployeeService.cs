using BusinessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Model.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeInfo>> GetAllEmployeesAsync();
        Task<EmployeeInfo> GetEmployeeByCodeAsync(string employeeCode);
        Task<ResultInfo> CreateEmployeeAsync(EmployeeInfo employeeInfo);
        Task<ResultInfo> UpdateEmployeeByCodeAsync(string employeeCode, EmployeeInfo employeeInfo);
        Task<ResultInfo> DeleteEmployeeByCodeAsync(string employeeCode);
    }
}
