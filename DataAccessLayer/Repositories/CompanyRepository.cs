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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbWrapper<Company> _companyDbWrapper;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly AsyncRetryPolicy _retryPolicy;

        public CompanyRepository(IDbWrapper<Company> companyDbWrapper)
        {
            _companyDbWrapper = companyDbWrapper ?? throw new ArgumentNullException(nameof(companyDbWrapper));

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

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await ExecuteDbCallWithRetryAsync(
                () => _companyDbWrapper.FindAllAsync(),
                "Exception occurred while fetching all companies."
            );
        }

        public async Task<Company> GetCompanyByCompanyCodeAsync(string companyCode)
        {
            return await ExecuteDbCallWithRetryAsync(
                async () =>
                {
                    var companies = await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode));
                    return companies?.FirstOrDefault();
                },
                $"Exception occurred while fetching company using companyCode: {companyCode}"
            );
        }

        public Task<DbResultInfo> CreateCompanyAsync(Company company)
        {
            return ExecuteDbCallWithRetryAsync(async () =>
            {
                var existingCompany = (await _companyDbWrapper.FindAsync(t => t.SiteId.Equals(company.SiteId) && t.CompanyCode.Equals(company.CompanyCode)))?.FirstOrDefault();
                if (existingCompany != null)
                {
                    return new DbResultInfo { IsSuccess = false, Message = "Company already found with the same Companycode." };
                }

                var insertResult = await _companyDbWrapper.InsertAsync(company);
                return new DbResultInfo { IsSuccess = insertResult, Message = insertResult ? "Company details saved successfully." : "Failed to save company details." };
            }, $"Exception occurred while saving company details: {company}");
        }

        public Task<DbResultInfo> UpdateByCodeAsync(string companyCode, Company company)
        {
            return ExecuteDbCallWithRetryAsync(async () =>
            {
                var existingCompany = (await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)))?.FirstOrDefault();
                if (existingCompany == null)
                {
                    return new DbResultInfo { IsSuccess = false, Message = "Company not found with the given company code." };
                }

                if (Helper.AreEntitiesEqual(existingCompany, company))
                {
                    return new DbResultInfo { IsSuccess = true, Message = "Same company details already exists." };
                }

                Helper.UpdateEntity(existingCompany, company);
                var updateResult = await _companyDbWrapper.UpdateAsync(existingCompany);

                return new DbResultInfo { IsSuccess = updateResult, Message = updateResult ? "Company details updated successfully." : "Failed to update company details." };
            }, $"Exception occurred while updating company using company code: {companyCode}");
        }

        public Task<bool> DeleteByCodeAsync(string companyCode)
        {
            return ExecuteDbCallWithRetryAsync(async () =>
            {
                var existingCompany = (await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)))?.FirstOrDefault();
                if (existingCompany == null)
                {
                    _logger.Warn($"Company with company code {companyCode} not available.");
                    return false;
                }

                var deleteResult = await _companyDbWrapper.DeleteAsync(t => t.CompanyCode.Equals(companyCode));
                return deleteResult;
            }, $"Exception occurred while deleting company using company code: {companyCode}");
        }

        public Task<Company> GetCompanyByCompanyNameAsync(string companyName)
        {
            return ExecuteDbCallWithRetryAsync(
                async () =>
                {
                    var companies = await _companyDbWrapper.FindAsync(t => t.CompanyName.Equals(companyName));
                    return companies?.FirstOrDefault();
                },
                $"Exception occurred while fetching company by company name: {companyName}"
            );
        }

    }
}
