using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using BusinessLayer.Services;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Tests
{
    [TestFixture]
    public class EmployeeServiceTests
    {
        private IEmployeeRepository _mockEmployeeRepository;
        private IMapper _mockMapper;
        private EmployeeService _employeeService;

        [SetUp]
        public void SetUp()
        {
            _mockEmployeeRepository = Substitute.For<IEmployeeRepository>();
            _mockMapper = Substitute.For<IMapper>();
            _employeeService = new EmployeeService(_mockEmployeeRepository, _mockMapper);
        }

        [Test]
        public async Task GetAllEmployeesAsync_ShouldReturnMappedEmployees()
        {
            // Arrange
            var employees = new List<Employee> { new Employee { Code = "E1" } };
            var employeeInfos = new List<EmployeeInfo> { new EmployeeInfo { Code = "E1" } };
            _mockEmployeeRepository.GetAllEmployeesAsync().Returns(Task.FromResult(employees));
            _mockMapper.Map<IEnumerable<EmployeeInfo>>(employees).Returns(employeeInfos);

            // Act
            var result = await _employeeService.GetAllEmployeesAsync();

            // Assert
            result.Should().BeEquivalentTo(employeeInfos);
        }

        [Test]
        public async Task GetEmployeeByCodeAsync_ShouldReturnMappedEmployee_WhenFound()
        {
            // Arrange
            var employeeCode = "E1";
            var employee = new Employee { Code = employeeCode };
            var employeeInfo = new EmployeeInfo { Code = employeeCode };
            _mockEmployeeRepository.GetEmployeeByCodeAsync(employeeCode).Returns(Task.FromResult(employee));
            _mockMapper.Map<EmployeeInfo>(employee).Returns(employeeInfo);

            // Act
            var result = await _employeeService.GetEmployeeByCodeAsync(employeeCode);

            // Assert
            result.Should().Be(employeeInfo);
        }

        [Test]
        public async Task GetEmployeeByCodeAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var employeeCode = "E1";
            _mockEmployeeRepository.GetEmployeeByCodeAsync(employeeCode).Returns(Task.FromResult((Employee)null));

            // Act
            var result = await _employeeService.GetEmployeeByCodeAsync(employeeCode);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task CreateEmployeeAsync_ShouldCreateEmployeeSuccessfully()
        {
            // Arrange
            var employeeInfo = new EmployeeInfo { Code = "E1" };
            var employee = new Employee { Code = "E1" };
            var resultInfo = new ResultInfo { IsSuccess = true, Message = "Employee created successfully." };

            _mockMapper.Map<Employee>(employeeInfo).Returns(employee);
            _mockEmployeeRepository.CreateEmployeeAsync(employee).Returns(Task.FromResult(resultInfo));

            // Act
            var result = await _employeeService.CreateEmployeeAsync(employeeInfo);

            // Assert
            result.Should().BeEquivalentTo(resultInfo);
        }

        [Test]
        public async Task UpdateEmployeeByCodeAsync_ShouldUpdateEmployeeSuccessfully_WhenFound()
        {
            // Arrange
            var employeeCode = "E1";
            var employeeInfo = new EmployeeInfo { Code = employeeCode };
            var employee = new Employee { Code = employeeCode };
            var updateResult = new ResultInfo { IsSuccess = true, Message = "Employee updated successfully." };

            _mockMapper.Map<Employee>(employeeInfo).Returns(employee);
            _mockEmployeeRepository.UpdateByCodeAsync(employeeCode, employee).Returns(Task.FromResult(updateResult));

            // Act
            var result = await _employeeService.UpdateEmployeeByCodeAsync(employeeCode, employeeInfo);

            // Assert
            result.Should().BeEquivalentTo(updateResult);
        }

        [Test]
        public async Task UpdateEmployeeByCodeAsync_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            var employeeCode = "E1";
            var employeeInfo = new EmployeeInfo { Code = employeeCode };
            var employee = new Employee { Code = employeeCode };
            var updateResult = new ResultInfo { IsSuccess = false, Message = "Failed to update employee." };

            _mockMapper.Map<Employee>(employeeInfo).Returns(employee);
            _mockEmployeeRepository.UpdateByCodeAsync(employeeCode, employee).Returns(Task.FromResult(updateResult));

            // Act
            var result = await _employeeService.UpdateEmployeeByCodeAsync(employeeCode, employeeInfo);

            // Assert
            result.Should().BeEquivalentTo(updateResult);
        }

        [Test]
        public async Task DeleteEmployeeByCodeAsync_ShouldDeleteSuccessfully_WhenFound()
        {
            // Arrange
            var employeeCode = "E1";
            _mockEmployeeRepository.DeleteByCodeAsync(employeeCode).Returns(Task.FromResult(true));

            // Act
            var result = await _employeeService.DeleteEmployeeByCodeAsync(employeeCode);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be($"Employee with code {employeeCode} was successfully deleted.");
        }

        [Test]
        public async Task DeleteEmployeeByCodeAsync_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            var employeeCode = "E1";
            _mockEmployeeRepository.DeleteByCodeAsync(employeeCode).Returns(Task.FromResult(false));

            // Act
            var result = await _employeeService.DeleteEmployeeByCodeAsync(employeeCode);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Employee not found.");
        }
    }
}