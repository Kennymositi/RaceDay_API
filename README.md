# RaceDay_API
RaceDay_API

Project Overview

RaceDay_API is an ASP.NET Core Web API built with .NET 8 and SQL Server to manage road-running events, participants, event categories, enrolments, and race results.

Technologies Used

- C#
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQL Server
- JWT Authentication
- xUnit unit testing
- GitHub Actions CI/CD

User Roles

- ORGANISER: Can manage events and record or update results for their own events.
- PARTICIPANT: Can view their own results and access participant functionality, including enrolment where supported by the API.

Authentication

The API uses JWT bearer authentication. Authenticated requests must include a valid token in the Authorization header:

"Authorization: Bearer <your-token>"

Role-based authorization restricts access to protected endpoints.

Database Setup

1. Install SQL Server.
2. Create or restore the "RaceDay_SA" database.
3. Update the "RaceDayConnection" connection string in your configuration to match your SQL Server instance.
4. Run the API from Visual Studio.

Running the API

1. Open "RaceDay_API.sln" in Visual Studio.
2. Restore NuGet packages.
3. Build the solution.
4. Run the "RaceDay_API" project.
5. Open the Swagger UI URL shown by the application to explore the endpoints.

Unit Tests

The solution includes an xUnit test project named "RaceDay_API.Tests".

Run the tests in Visual Studio Test Explorer, or use:

dotnet test RaceDay_API.sln

Continuous Integration (CI/CD)

GitHub Actions automatically builds the solution and runs the unit tests when changes are pushed to the repository.

Successful GitHub Actions Build

Add a screenshot of the successful GitHub Actions workflow run here.

Repository

"RaceDay_API on GitHub" (https://github.com/Kennymositi/RaceDay_API)
