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
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<CompanyInfo>> GetAllCompaniesAsync()
        {
            return await ExecuteAsync(async () =>
            {
                var companies = await _companyRepository.GetAllCompaniesAsync();
                return _mapper.Map<IEnumerable<CompanyInfo>>(companies);
            });
        }
        public async Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode)
        {
            return await ExecuteAsync(async () =>
            {
                var company = await _companyRepository.GetCompanyByCompanyCodeAsync(companyCode);
                if (company == null)
                {
                    _logger.Warn($"Company with code {companyCode} not found.");
                    return null;
                }
                return _mapper.Map<CompanyInfo>(company);
            });
        }

        public async Task<ResultInfo> CreateCompanyAsync(CompanyInfo companyInfo)
        {
            return await ExecuteAsync(async () =>
            {
                var company = _mapper.Map<Company>(companyInfo);
                var saveResult = await _companyRepository.CreateCompanyAsync(company);
                return new ResultInfo { IsSuccess = saveResult.IsSuccess, Message = saveResult.Message };
            });
        }

        public async Task<ResultInfo> UpdateCompanyByCodeAsync(string companyCode, CompanyInfo companyInfo)
        {
            return await ExecuteAsync(async () =>
            {
                var company = _mapper.Map<Company>(companyInfo);
                var updateResult = await _companyRepository.UpdateByCodeAsync(companyCode, company);
                if (!updateResult.IsSuccess)
                {
                    _logger.Warn($"Company with code {companyCode} not found.");
                    return new ResultInfo { IsSuccess = false, Message = "Failed to update company." };
                }
                return new ResultInfo { IsSuccess = true, Message = "Company updated successfully." };
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
                _logger.Error($"CompanyService: An error occurred while processing the request - {ex.Message} :: {ex.InnerException.Message} ");
                return default;
            }
        }

        public async Task<ResultInfo> DeleteCompanyByCodeAsync(string companyCode)
        {

            var result = await _companyRepository.DeleteByCodeAsync(companyCode);
            if (!result)
            {
                _logger.Warn($"Company with code {companyCode} not found.");
                return new ResultInfo { IsSuccess = false, Message = "Company not found." };
            }

            return new ResultInfo { IsSuccess = true, Message = $"Company with code {companyCode} was successfully deleted." };
        }
    }
}
