using AutoMapper;
using BusinessLayer.Model.Models;
using BusinessLayer.Services;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class CompanyServiceTests
    {
        private ICompanyRepository _mockCompanyRepository;
        private IMapper _mockMapper;
        private CompanyService _companyService;

        [SetUp]
        public void SetUp()
        {
            _mockCompanyRepository = Substitute.For<ICompanyRepository>();
            _mockMapper = Substitute.For<IMapper>();
            _companyService = new CompanyService(_mockCompanyRepository, _mockMapper);
        }

        [Test]
        public async Task GetAllCompaniesAsync_ReturnsMappedCompanies()
        {
            // Arrange
            var companies = new List<Company> { new Company { CompanyCode = "C1" } };
            var companyInfos = new List<CompanyInfo> { new CompanyInfo { CompanyCode = "C1" } };
            _mockCompanyRepository.GetAllCompaniesAsync().Returns((IEnumerable<Company>)Task.FromResult(companies));
            _mockMapper.Map<IEnumerable<CompanyInfo>>(companies).Returns(companyInfos);

            // Act
            var result = await _companyService.GetAllCompaniesAsync();

            // Assert
            result.Should().BeEquivalentTo(companyInfos);
        }

        [Test]
        public async Task GetCompanyByCodeAsync_ReturnsMappedCompany_WhenFound()
        {
            // Arrange
            var companyCode = "C1";
            var company = new Company { CompanyCode = companyCode };
            var companyInfo = new CompanyInfo { CompanyCode = companyCode };
            _mockCompanyRepository.GetCompanyByCompanyCodeAsync(companyCode).Returns(Task.FromResult(company));
            _mockMapper.Map<CompanyInfo>(company).Returns(companyInfo);

            // Act
            var result = await _companyService.GetCompanyByCodeAsync(companyCode);

            // Assert
            result.Should().Be(companyInfo);
        }

        [Test]
        public async Task GetCompanyByCodeAsync_ReturnsNull_WhenNotFound()
        {
            // Arrange
            var companyCode = "C1";
            _mockCompanyRepository.GetCompanyByCompanyCodeAsync(companyCode).Returns(Task.FromResult((Company)null));

            // Act
            var result = await _companyService.GetCompanyByCodeAsync(companyCode);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task CreateCompanyAsync_CreatesCompanySuccessfully()
        {
            // Arrange
            var companyInfo = new CompanyInfo { CompanyCode = "C1" };
            var company = new Company { CompanyCode = "C1" };
            var resultInfo = new DbResultInfo { IsSuccess = true, Message = "Created successfully." };
            _mockMapper.Map<Company>(companyInfo).Returns(company);
            _mockCompanyRepository.CreateCompanyAsync(company).Returns(Task.FromResult(resultInfo));

            // Act
            var result = await _companyService.CreateCompanyAsync(companyInfo);

            // Assert
            result.Should().BeEquivalentTo(resultInfo);
        }

        [Test]
        public async Task UpdateCompanyByCodeAsync_UpdatesCompanySuccessfully_WhenFound()
        {
            // Arrange
            var companyCode = "C1";
            var companyInfo = new CompanyInfo { CompanyCode = companyCode };
            var company = new Company { CompanyCode = companyCode };
            var updateResult = new DbResultInfo { IsSuccess = true, Message = "Update successful." };
            _mockMapper.Map<Company>(companyInfo).Returns(company);
            _mockCompanyRepository.UpdateByCodeAsync(companyCode, company).Returns(Task.FromResult(updateResult));

            // Act
            var result = await _companyService.UpdateCompanyByCodeAsync(companyCode, companyInfo);

            // Assert
            result.Should().BeEquivalentTo(updateResult);
        }

        [Test]
        public async Task DeleteCompanyByCodeAsync_DeletesSuccessfully_WhenFound()
        {
            // Arrange
            var companyCode = "C1";
            _mockCompanyRepository.DeleteByCodeAsync(companyCode).Returns(Task.FromResult(true));

            // Act
            var result = await _companyService.DeleteCompanyByCodeAsync(companyCode);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public async Task DeleteCompanyByCodeAsync_ReturnsFailure_WhenNotFound()
        {
            // Arrange
            var companyCode = "C1";
            _mockCompanyRepository.DeleteByCodeAsync(companyCode).Returns(Task.FromResult(false));

            // Act
            var result = await _companyService.DeleteCompanyByCodeAsync(companyCode);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Company not found.");
        }
    }
}