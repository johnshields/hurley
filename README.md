# HurleyAPI 🐛

A C#/.NET issue-tracking API using Supabase (PostgreSQL) as the backend database. Features Dapper ORM integration for efficient SQL queries, full CRUD functionality, and LINQ-powered filter endpoints and tested via xUnit.

## 📁 API Directory & File Structure
```
src/
├── Models/                # DTOs & enums
│   └── Enums.cs
│   └── IssueDto.cs
│   └── IssueReport.cs
├── Services/              # Core business logic
│   └── IssueService.cs
├── Tests/                 # xUnit tests for IssueService
│   └── IssueServiceTests.cs
├── Program.cs             # API entry point and route definitions
├── appsettings.json       # Configuration (unused for Supabase)
└── HurleyAPI.csproj       # Project file
```
---

#### Requirements

- [Git](https://git-scm.com/downloads)
- [.NET SDK 9.0.200](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Supabase](https://supabase.com/)
- [Postman](https://www.postman.com/downloads/) _(for manual API testing)_

## Run service:

#### Open a directory in Command-Line and enter:
```bash
$ git clone https://github.com/johnshields/hurley.git
$ cd hurley/
```

#### Add Supabase credentials to [launchSettings.json](Properties/launchSettings.json) under environmentVariables:
```json
"SUPABASE_URL": "your_supabase_url",
"SUPABASE_KEY": "your_supabase_anon_key"
```

#### Add Supabase credentials to `IssueServiceTests.cs`:
```csharp
var url = "your_supabase_url";
var key = "your_supabase_anon_key";
```

#### SQL script located here [works/sql/hurley_db.sql](works/sql/hurley_db.sql)

#### Run the API

```bash
$ dotnet clean
$ dotnet build
$ dotnet test    # run all xUnit tests
$ dotnet run     # run the API
```

- The API will listen on: http://localhost:5147/
- View API Swagger docs: http://localhost:5147/swagger/index.html

---

## 📦 API Endpoints

- `GET /api/issues` – List all issues (supports optional filters)
- `GET /api/issues/{id}` – Get a single issue by ID
- `POST /api/issues` – Insert a new issue
- `PUT /api/issues/{id}` – Update an issue by ID
- `DELETE /api/issues/{id}` – Delete an issue by ID

---
