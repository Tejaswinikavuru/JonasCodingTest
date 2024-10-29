using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccessLayer.Model.Models;

namespace DataAccessLayer.Model.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task<Company> GetCompanyByCompanyCodeAsync(string companyCode);
        Task<Company> GetCompanyByCompanyNameAsync(string companyName);
        Task<DbResultInfo> CreateCompanyAsync(Company company);
        Task<DbResultInfo> UpdateByCodeAsync(string companyCode, Company company);
        Task<bool> DeleteByCodeAsync(string companyCode);
    }
}
