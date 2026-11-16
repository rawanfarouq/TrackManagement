# Track Management System

A full-stack music track management application developed for Takwene's Full-Stack Software Engineer technical assessment.

## Tech Stack

- Backend: ASP.NET Core Web API (.NET 10)
- Frontend: React + TypeScript (planned)
- Database: Microsoft SQL Server (planned)
- ORM: Entity Framework Core (planned)
- Authentication: JWT (planned)

## Architecture

The backend follows Clean Architecture, divided into four projects:

- **Domain:** Core entities and business rules.
- **Application:** Use cases, services, interfaces, and DTOs.
- **Infrastructure:** Database access and EF Core implementation.
- **Api:** REST endpoints, controllers, and authentication.


## Setup and Running Instructions

Detailed setup, database migrations, API execution, frontend execution, and JWT token instructions will be added during implementation.

## Database Setup

The application uses Microsoft SQL Server with Entity Framework Core 10.

### Local Configuration

The development database uses SQL Server Express:

- Server: `localhost\SQLEXPRESS`
- Database: `TrackManagementDb`
- Authentication: Windows Authentication

Update the `DefaultConnection` connection string in `backend/TrackManagement.Api/appsettings.json` for your local environment. The example configuration uses `TrustServerCertificate=True` for local development only.

### Install EF Core CLI Tools

From the repository root:

```bash
dotnet tool restore
```

### Apply Database Migrations

```bash
dotnet ef database update --project backend/TrackManagement.Infrastructure --startup-project backend/TrackManagement.Api --context TrackManagementDbContext
```

This creates the database and its tables if they do not already exist.

### Run the Backend

```bash
dotnet run --project backend/TrackManagement.Api
```

In the Development environment, the application seeds the empty database with:

- 3 artists
- 8 tracks across different genres and statuses
- 3 DSPs (Spotify, Apple Music, YouTube)
- 7 track distribution records

Sample data is inserted automatically when the API starts against an empty database.

## Implementation Progress

- [x] Clean Architecture project setup
- [x] Domain entities and enums
- [x] SQL Server integration using EF Core
- [x] Initial database migration
- [x] Development database seeding
- [ ] Application services and DTOs
- [ ] REST API endpoints
- [ ] JWT authentication
- [ ] React frontend
- [ ] Final testing and documentation