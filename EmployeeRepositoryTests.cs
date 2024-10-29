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
    public class EmployeeRepositoryTests
    {
        private IEmployeeRepository _employeeRepository;
        private IDbWrapper<Employee> _mockDbWrapper;

        [SetUp]
        public void SetUp()
        {
            _mockDbWrapper = Substitute.For<IDbWrapper<Employee>>();
            _employeeRepository = new EmployeeRepository(_mockDbWrapper);
        }

        [Test]
        public async Task GetAllEmployeesAsync_ShouldReturnAllEmployees()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { EmployeeCode = "E1" },
                new Employee { EmployeeCode = "E2" }
            };
            _mockDbWrapper.FindAllAsync().Returns(Task.FromResult(employees));

            // Act
            var result = await _employeeRepository.GetAllEmployeesAsync();

            // Assert
            result.Should().BeEquivalentTo(employees);
        }

        [Test]
        public async Task GetEmployeeByCodeAsync_ShouldReturnEmployee_WhenFound()
        {
            // Arrange
            var employeeCode = "E1";
            var employee = new Employee { EmployeeCode = employeeCode };
            _mockDbWrapper.FindAsync(Arg.Any<Func<Employee, bool>>()).Returns(Task.FromResult(new List<Employee> { employee }));

            // Act
            var result = await _employeeRepository.GetEmployeeByCodeAsync(employeeCode);

            // Assert
            result.Should().Be(employee);
        }

        [Test]
        public async Task GetEmployeeByCodeAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var employeeCode = "E1";
            _mockDbWrapper.FindAsync(Arg.Any<Func<Employee, bool>>()).Returns(Task.FromResult(new List<Employee>()));

            // Act
            var result = await _employeeRepository.GetEmployeeByCodeAsync(employeeCode);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task CreateEmployeeAsync_ShouldReturnSuccess_WhenEmployeeIsCreated()
        {
            // Arrange
            var employee = new Employee { EmployeeCode = "E1", SiteId = 1 };
            _mockDbWrapper.FindAsync(Arg.Any<Func<Employee, bool>>()).Returns(Task.FromResult(new List<Employee>()));
            _mockDbWrapper.InsertAsync(employee).Returns(Task.FromResult(true));

            // Act
            var result = await _employeeRepository.CreateEmployeeAsync(employee);

            // Assert
            result.Should().BeEquivalentTo(new DbResultInfo { IsSuccess = true, Message = "Employee details saved successfully." });
        }

        [Test]
        public async Task CreateEmployeeAsync_ShouldReturnFailure_WhenEmployeeAlreadyExists()
        {
            // Arrange
            var employee = new Employee { EmployeeCode = "E1", SiteId = 1 };
            var existingEmployee = new Employee { EmployeeCode = "E1", SiteId = 1 };
            _mockDbWrapper.FindAsync(Arg.Any<Func<Employee, bool>>()).Returns(Task.FromResult(new List<Employee> { existingEmployee }));

            // Act
            var result = await _employeeRepository.CreateEmployeeAsync(employee);

            // Assert
            result.Should().BeEquivalentTo(new DbResultInfo { IsSuccess = false, Message = "Employee already exists with the same EmployeeCode." });
        }

        [Test]
        public async Task UpdateByCodeAsync_ShouldReturnSuccess_WhenUpdatedSuccessfully()
        {
            // Arrange
            var employeeCode = "E1";
            var existingEmployee = new Employee { EmployeeCode = employeeCode };
            var updatedEmployee = new Employee { EmployeeCode = employeeCode };
            _mockDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)).Returns(Task.FromResult(new List<Employee> { existingEmployee }));
            _mockDbWrapper.UpdateAsync(existingEmployee).Returns(Task.FromResult(true));

            // Act
            var result = await _employeeRepository.UpdateByCodeAsync(employeeCode, updatedEmployee);

            // Assert
            result.Message.Should().Be("Employee details updated successfully.");
            result.IsSuccess.Should().BeTrue();
        }

        [Test]
        public async Task UpdateByCodeAsync_ShouldReturnFailure_WhenEmployeeNotFound()
        {
            // Arrange
            var employeeCode = "E1";
            var updatedEmployee = new Employee { EmployeeCode = employeeCode };
            _mockDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)).Returns(Task.FromResult(new List<Employee>()));

            // Act
            var result = await _employeeRepository.UpdateByCodeAsync(employeeCode, updatedEmployee);

            // Assert
            result.Should().BeEquivalentTo(new DbResultInfo { IsSuccess = false, Message = "Employee not found with the given employee code." });
        }

        [Test]
        public async Task DeleteByCodeAsync_ShouldReturnTrue_WhenEmployeeDeletedSuccessfully()
        {
            // Arrange
            var employeeCode = "E1";
            var existingEmployee = new Employee { EmployeeCode = employeeCode };
            _mockDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)).Returns(Task.FromResult(new List<Employee> { existingEmployee }));
            _mockDbWrapper.DeleteAsync(Arg.Any<Func<Employee, bool>>()).Returns(Task.FromResult(true));

            // Act
            var result = await _employeeRepository.DeleteByCodeAsync(employeeCode);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task DeleteByCodeAsync_ShouldReturnFalse_WhenEmployeeNotFound()
        {
            // Arrange
            var employeeCode = "E1";
            _mockDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)).Returns(Task.FromResult(new List<Employee>()));

            // Act
            var result = await _employeeRepository.DeleteByCodeAsync(employeeCode);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task GetEmployeeByNameAsync_ShouldReturnEmployee_WhenFound()
        {
            // Arrange
            var employeeName = "John Doe";
            var employee = new Employee { EmployeeName = employeeName };
            _mockDbWrapper.FindAsync(t => t.EmployeeName.Equals(employeeName)).Returns(Task.FromResult(new List<Employee> { employee }));

            // Act
            var result = await _employeeRepository.GetEmployeeByNameAsync(employeeName);

            // Assert
            result.Should().Be(employee);
        }

        [Test]
        public async Task GetEmployeeByNameAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var employeeName = "John Doe";
            _mockDbWrapper.FindAsync(t => t.EmployeeName.Equals(employeeName)).Returns(Task.FromResult(new List<Employee>()));

            // Act
            var result = await _employeeRepository.GetEmployeeByNameAsync(employeeName);

            // Assert
            result.Should().BeNull();
        }
    }
}