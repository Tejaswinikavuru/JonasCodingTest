using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.Model.Models;

namespace BusinessLayer.Model.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyInfo>> GetAllCompaniesAsync();
        Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode);
        Task<ResultInfo> CreateCompanyAsync(CompanyInfo companyInfo);
        Task<ResultInfo> UpdateCompanyByCodeAsync(string companyCode, CompanyInfo companyInfo);
        Task<ResultInfo> DeleteCompanyByCodeAsync(string companyCode);
    }
}
