# HurleyAPI

A RESTful issue-tracking API for managing issues across teams and projects.

## Development Environment

### HurleyAPI
- **.NET SDK** - 9.0.200
- **Framework** - ASP.NET Core Minimal API
- **Unit Testing** - xUnit

### Tools
- **OS** - Windows 11
- **IDE** - JetBrains Rider 2025.1.3
- **API Testing** - Postman

## API Directory & File Structure

```
HurleyAPI              # Main API project
│   Program.cs         # Application entry point
│   appsettings.json   # Configuration file
└───Data/              # JSON data storage
└───Models/            # DTOs & enums
└───Services/          # Business logic and API route registrations
└───Tests/             # xUnit unit tests for IssueService
```

## How to run

### Requirements

* [Git](https://git-scm.com/downloads)
* [.NET SDK 9.0.200](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* [xUnit](https://xunit.net/)
* [Postman](https://www.postman.com/downloads/) _(for manual API testing)_

#### Open a directory in Command-Line and enter:
```bash
$ git clone https://github.com/johnshields/HurleyAPI
$ cd HurleyAPI/
$ dotnet clean
$ dotnet build
$ dotnet test # run xUnit tests
$ dotnet run # run API
```
* The API will listen on: http://localhost:5147/
* Swagger UI: https://localhost:5147/swagger/index.html
***
