# .NET Web API - JonasCodingTest

This project is a simple .NET Web API designed to manage company and employee information. It demonstrates key concepts like asynchronous programming, repository pattern, proper error handling with logging, and separation of concerns across different layers.

## Table of Contents
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Setup and Installation](#setup-and-installation)
- [Layers Description](#layers-description)
- [Features](#features)
- [Swagger](#swagger)
- [API Endpoints](#api-endpoints)
- [Error Handling and Logging](#error-handling-and-logging)


## Technologies Used

- **.NET Framework 4.7.2**
- **Web API**
- **C#**
- **Serilog**

## Prerequisites

- **Visual Studio 2019 or higher**
- **.NET Framework 4.7.2 SDK**
- **Postman**

## Setup and Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/baqarmangrani/JonasCodingTest
   cd JonasCodingTest
   ```

2. **Open Solution**

   Open `JonasCodingTest.sln` using Visual Studio.

3. **Build the Solution**

   Ensure all projects compile successfully.

4. **Run the Web API**

   Set `WebApi` as the startup project and run it.

5. **Import the Postman Collection**

   Call the endpoints

## Layers Description

- **BusinessLayer.Model**: Contains business-specific business models and Interfaces.
- **BusinessLayer**: Contains service classes responsible for business logic, working, Exceptions and mapping data models to business models.
- **DataLayer.Model**: Defines database entities and interfaces for repositories.
- **DataAccessLayer**: Contains InMemoryDatabase and implements repository interfaces to perform CRUD operations on the in-memory database.
- **WebApi**: Hosts the API controllers and manages dependency injection, routing, and configuration.

## Features

- **Implemented Company Controller Functions**: Completed all CRUD operations for the Company controller, ensuring full integration down to the data access layer.
- **Asynchronous Company Controller**: Refactored all Company controller functions to be asynchronous, improving performance and scalability.
- **Employee Repository**: Created a new repository to manage employee information with the following data model properties:
  - `string SiteId`
  - `string CompanyCode`
  - `string EmployeeCode`
  - `string EmployeeName`
  - `string Occupation`
  - `string EmployeeStatus`
  - `string EmailAddress`
  - `string Phone`
  - `DateTime LastModified`
- **Employee Controller**: Developed a new controller to retrieve employee information for the client side with the following properties:
  - `string EmployeeCode`
  - `string EmployeeName`
  - `string CompanyName`
  - `string OccupationName`
  - `string EmployeeStatus`
  - `string EmailAddress`
  - `string PhoneNumber`
  - `string LastModifiedDateTime`
- **Logging and Error Handling**: Integrated a logging framework (Nlog) into the solution and implemented comprehensive error handling to ensure all exceptions are logged and user-friendly responses are returned.
## Swagger
  Implemented Swagger using SwashBuckle package and when you run the solution append swagger to the localhost uri.

  ![image](https://github.com/user-attachments/assets/458ba64a-04ff-4158-9469-1b635b530109)

## API Endpoints

### Companies Controller

The `CompaniesController` provides endpoints to manage company information. Below are the available API endpoints:

### Employees Controller

The `EmployeesController` provides endpoints to manage employee information. Below are the available API endpoints:


## Error Handling and Logging

The solution uses a logging framework i.e. nlog to capture and log errors. Implemented Global Exception handler, ensuring that every exception is logged, and the API returns a user-friendly response with relevant status codes.

The solution uses polly for relisience of API's like use retrylogic from Polly.
