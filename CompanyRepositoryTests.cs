using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using DataAccessLayer.Repositories;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Tests
{
	[TestFixture]
	public class CompanyRepositoryTests
	{
		private ICompanyRepository _companyRepository;
		private IDbWrapper<Company> _mockDbWrapper;

		[SetUp]
		public void SetUp()
		{
			_mockDbWrapper = Substitute.For<IDbWrapper<Company>>();
			_companyRepository = new CompanyRepository(_mockDbWrapper);
		}

		[Test]
		public async Task GetAllCompaniesAsync_ShouldReturnAllCompanies()
		{
			// Arrange
			var companies = new List<Company>
			{
				new Company { CompanyCode = "C1" },
				new Company { CompanyCode = "C2" }
			};
			_mockDbWrapper.FindAllAsync().Returns(Task.FromResult(companies));

			// Act
			var result = await _companyRepository.GetAllCompaniesAsync();

			// Assert
			result.Should().BeEquivalentTo(companies);
		}

		[Test]
		public async Task GetCompanyByCompanyCodeAsync_ShouldReturnCompany_WhenFound()
		{
			// Arrange
			var companyCode = "C1";
			var company = new Company { CompanyCode = companyCode };
			_mockDbWrapper.FindAsync(Arg.Any<Func<Company, bool>>()).Returns(Task.FromResult(new List<Company> { company }));

			// Act
			var result = await _companyRepository.GetCompanyByCompanyCodeAsync(companyCode);

			// Assert
			result.Should().Be(company);
		}

		[Test]
		public async Task GetCompanyByCompanyCodeAsync_ShouldReturnNull_WhenNotFound()
		{
			// Arrange
			var companyCode = "C1";
			_mockDbWrapper.FindAsync(Arg.Any<Func<Company, bool>>()).Returns(Task.FromResult(new List<Company>()));

			// Act
			var result = await _companyRepository.GetCompanyByCompanyCodeAsync(companyCode);

			// Assert
			result.Should().BeNull();
		}

		[Test]
		public async Task CreateCompanyAsync_ShouldReturnSuccess_WhenCompanyIsCreated()
		{
			// Arrange
			var company = new Company { CompanyCode = "C1", SiteId = 1 };
			_mockDbWrapper.FindAsync(Arg.Any<Func<Company, bool>>()).Returns(Task.FromResult(new List<Company>()));
			_mockDbWrapper.InsertAsync(company).Returns(Task.FromResult(true));

			// Act
			var result = await _companyRepository.CreateCompanyAsync(company);

			// Assert
			result.Should().BeEquivalentTo(new DbResultInfo { IsSuccess = true, Message = "Company details saved successfully." });
		}

		[Test]
		public async Task CreateCompanyAsync_ShouldReturnFailure_WhenCompanyAlreadyExists()
		{
			// Arrange
			var company = new Company { CompanyCode = "C1", SiteId = 1 };
			var existingCompany = new Company { CompanyCode = "C1", SiteId = 1 };
			_mockDbWrapper.FindAsync(Arg.Any<Func<Company, bool>>()).Returns(Task.FromResult(new List<Company> { existingCompany }));

			// Act
			var result = await _companyRepository.CreateCompanyAsync(company);

			// Assert
			result.Should().BeEquivalentTo(new DbResultInfo { IsSuccess = false, Message = "Company already found with the same Companycode." });
		}

		[Test]
		public async Task UpdateByCodeAsync_ShouldReturnSuccess_WhenUpdatedSuccessfully()
		{
			// Arrange
			var companyCode = "C1";
			var existingCompany = new Company { CompanyCode = companyCode };
			var updatedCompany = new Company { CompanyCode = companyCode };
			_mockDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)).Returns(Task.FromResult(new List<Company> { existingCompany }));
			_mockDbWrapper.UpdateAsync(existingCompany).Returns(Task.FromResult(true));

			// Act
			var result = await _companyRepository.UpdateByCodeAsync(companyCode, updatedCompany);

			// Assert
			result.Message.Should().Be("Company details updated successfully.");
			result.IsSuccess.Should().BeTrue();
		}

		[Test]
		public async Task UpdateByCodeAsync_ShouldReturnFailure_WhenCompanyNotFound()
		{
			// Arrange
			var companyCode = "C1";
			var updatedCompany = new Company { CompanyCode = companyCode };
			_mockDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)).Returns(Task.FromResult(new List<Company>()));

			// Act
			var result = await _companyRepository.UpdateByCodeAsync(companyCode, updatedCompany);

			// Assert
			result.Should().BeEquivalentTo(new DbResultInfo { IsSuccess = false, Message = "Company not found with the given company code." });
		}

		[Test]
		public async Task DeleteByCodeAsync_ShouldReturnTrue_WhenCompanyDeletedSuccessfully()
		{
			// Arrange
			var companyCode = "C1";
			var existingCompany = new Company { CompanyCode = companyCode };
			_mockDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)).Returns(Task.FromResult(new List<Company> { existingCompany }));
			_mockDbWrapper.DeleteAsync(Arg.Any<Func<Company, bool>>()).Returns(Task.FromResult(true));

			// Act
			var result = await _companyRepository.DeleteByCodeAsync(companyCode);

			// Assert
			result.Should().BeTrue();
		}

		[Test]
		public async Task DeleteByCodeAsync_ShouldReturnFalse_WhenCompanyNotFound()
		{
			// Arrange
			var companyCode = "C1";
			_mockDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)).Returns(Task.FromResult(new List<Company>()));

			// Act
			var result = await _companyRepository.DeleteByCodeAsync(companyCode);

			// Assert
			result.Should().BeFalse();
		}

		[Test]
		public async Task GetCompanyByCompanyNameAsync_ShouldReturnCompany_WhenFound()
		{
			// Arrange
			var companyName = "MyCompany";
			var company = new Company { CompanyName = companyName };
			_mockDbWrapper.FindAsync(t => t.CompanyName.Equals(companyName)).Returns(Task.FromResult(new List<Company> { company }));

			// Act
			var result = await _companyRepository.GetCompanyByCompanyNameAsync(companyName);

			// Assert
			result.Should().Be(company);
		}

		[Test]
		public async Task GetCompanyByCompanyNameAsync_ShouldReturnNull_WhenNotFound()
		{
			// Arrange
			var companyName = "MyCompany";
			_mockDbWrapper.FindAsync(t => t.CompanyName.Equals(companyName)).Returns(Task.FromResult(new List<Company>()));

			// Act
			var result = await _companyRepository.GetCompanyByCompanyNameAsync(companyName);

			// Assert
			result.Should().BeNull();
		}
	}
}