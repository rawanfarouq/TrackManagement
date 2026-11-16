# Track Management System

A full-stack music track management application developed for Takwene's Full-Stack Software Engineer technical assessment.

The system manages artists, music tracks, and their distribution to Digital Service Providers (DSPs) such as Spotify, Apple Music, and YouTube.

## Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Frontend:** React + TypeScript (planned)
- **Database:** Microsoft SQL Server 2022 Express
- **ORM:** Entity Framework Core 10
- **Authentication:** JWT (planned)
- **API Testing:** Postman
- **Version Control:** Git and GitHub

## Architecture

The backend follows Clean Architecture, divided into four projects:

- **Domain:** Core entities, relationships, and status enums.
- **Application:** Business logic, services, interfaces, DTOs, and input validation.
- **Infrastructure:** SQL Server database access, EF Core, repositories, migrations, and database seeding.
- **Api:** REST controllers, HTTP request handling, dependency injection, and authentication configuration.

### Backend Project Structure

```text
backend/
├── TrackManagement.Domain/
├── TrackManagement.Application/
├── TrackManagement.Infrastructure/
└── TrackManagement.Api/
```

## Setup and Running Instructions

### Prerequisites

- .NET 10 SDK
- Microsoft SQL Server Express
- SQL Server Management Studio (optional)
- EF Core CLI Tools
- Git
- Postman (for API testing)

### Database Setup

The application uses Microsoft SQL Server with Entity Framework Core.

**Local Configuration**

- Server: `localhost\SQLEXPRESS`
- Database: `TrackManagementDb`
- Authentication: Windows Authentication

Update the `DefaultConnection` connection string in `backend/TrackManagement.Api/appsettings.json` according to your local SQL Server configuration.

The development configuration uses `TrustServerCertificate=True` for the local SQL Server instance only.

### Restore Dependencies

From the repository root:

```bash
dotnet restore
dotnet tool restore
```

### Apply Database Migrations

```bash
dotnet ef database update --project backend/TrackManagement.Infrastructure --startup-project backend/TrackManagement.Api --context TrackManagementDbContext
```

This creates the database tables, relationships, indexes, and constraints if the migration has not already been applied.

### Build the Backend

```bash
dotnet build
```

### Run the Backend

```bash
dotnet run --project backend/TrackManagement.Api
```

The API has been tested locally at:

`http://localhost:5150`

The actual port may vary depending on the launch configuration.

### Database Seeding

In the Development environment, the application automatically seeds an initially empty database with:

- 3 artists
- 8 tracks across different genres and statuses
- 3 DSPs (Spotify, Apple Music, YouTube)
- 7 track distribution records

The seeding logic checks for existing data to avoid inserting duplicate sample records during normal application restarts.

## Implemented API Endpoints

| Method | Endpoint | Description | Status |
|---|---|---|---|
| POST | `/api/artists` | Create a new artist | Completed |
| GET | `/api/artists` | Retrieve all artists | Completed |
| POST | `/api/tracks` | Create a track for an existing artist | Completed |
| GET | `/api/tracks` | List tracks with optional filters | Completed |
| GET | `/api/tracks/{id}` | Retrieve track details and DSP distribution statuses | Completed |
| PATCH | `/api/tracks/{id}/status` | Update a track's status | Pending |
| POST | `/api/tracks/{id}/distribute` | Submit a track to one or more DSPs | Pending |

### Track Filtering

The Track List endpoint supports filtering by:

- `artistId`
- `genre`
- `status`

Example:

```http
GET /api/tracks?genre=Pop&status=distributed
```

### Input Validation

The API currently includes request validation for artists and tracks, including:

- Required fields and maximum lengths
- Email format validation
- ISRC format validation
- Artist existence checks
- Duplicate ISRC checks

Database-level unique indexes also help enforce data integrity.

Additional error handling and security testing will be completed during the remaining implementation.

## API Testing

The implemented endpoints have been tested using Postman.

Verified scenarios include:

- Retrieving existing artists and tracks
- Creating new artists and tracks
- Retrieving individual track details
- Retrieving DSP distribution statuses
- Applying track filters
- Rejecting invalid artist email input

Further endpoint and edge-case testing will be performed as development continues.

## Implementation Progress

- [x] Clean Architecture project setup
- [x] Domain entities and enums
- [x] SQL Server integration using EF Core
- [x] Initial database migration
- [x] Development database seeding
- [x] Artist DTOs, service, and repository
- [x] Artist API endpoints
- [x] Track DTOs, service, and repository
- [x] Track creation, listing, filtering, and details endpoints
- [x] Initial Postman testing
- [ ] Track status update endpoint
- [ ] Track distribution endpoint
- [ ] JWT authentication and authorization
- [ ] React frontend
- [ ] Final validation, security testing, and documentation

## Frontend

The React frontend will provide two main views:

1. **Track List:** Displays tracks, artist names, genres, and statuses, with filtering by status.
2. **Track Detail:** Displays track information and its distribution statuses across DSPs.

Frontend setup and execution instructions will be added after implementation.

## JWT Authentication

JWT authentication will be implemented to protect at least one API endpoint. Instructions for obtaining and using a JWT token will be added once authentication is complete.

## Development Decisions

See [DECISIONS.md](DECISIONS.md) for details about AI-assisted development, implementation decisions, security considerations, and issues identified and corrected during development.