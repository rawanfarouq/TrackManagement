# Track Management System

A full-stack music track management application developed for Takwene's Full-Stack Software Engineer technical assessment.

The system manages artists, music tracks, and their distribution to Digital Service Providers (DSPs), including Spotify, Apple Music, and YouTube.

## Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Frontend:** React, TypeScript, Vite
- **Frontend Styling:** CSS
- **Database:** Microsoft SQL Server 2022 Express
- **ORM:** Entity Framework Core 10
- **Authentication:** JWT Bearer authentication
- **API Testing:** Postman
- **Version Control:** Git and GitHub

## Architecture

The backend follows Clean Architecture and is divided into four projects:

- **Domain:** Core entities, relationships, and status enums.
- **Application:** Business logic, services, interfaces, DTOs, and input validation.
- **Infrastructure:** SQL Server database access, EF Core repositories, migrations, and database seeding.
- **API:** REST controllers, HTTP request handling, dependency injection, authentication, and CORS configuration.

### Project Structure

```text
TrackManagement/
├── backend/
│   ├── TrackManagement.Domain/
│   ├── TrackManagement.Application/
│   ├── TrackManagement.Infrastructure/
│   └── TrackManagement.Api/
├── frontend/
│   ├── src/
│   │   ├── services/
│   │   ├── types/
│   │   ├── App.tsx
│   │   ├── App.css
│   │   └── index.css
│   └── package.json
├── TrackManagement.slnx
├── README.md
└── DECISIONS.md
```

## Setup and Running Instructions

### Prerequisites

- .NET 10 SDK
- Node.js and npm
- Microsoft SQL Server Express
- SQL Server Management Studio (optional)
- EF Core CLI tools
- Git
- Postman (for API testing)

### 1. Database Setup

The application uses Microsoft SQL Server with Entity Framework Core.

**Local development configuration:**

- Server: `localhost\SQLEXPRESS`
- Database: `TrackManagementDb`
- Authentication: Windows Authentication

Update the `DefaultConnection` connection string in `backend/TrackManagement.Api/appsettings.json` to match your local SQL Server configuration.

The local development configuration uses `TrustServerCertificate=True` for the SQL Server Express instance. This is a development convenience, not a recommended production security setting.

### 2. Restore Backend Dependencies

From the repository root:

```bash
dotnet restore
dotnet tool restore
```

### 3. Apply Database Migrations

```bash
dotnet ef database update --project backend/TrackManagement.Infrastructure --startup-project backend/TrackManagement.Api --context TrackManagementDbContext
```

This applies the existing migration and creates the required tables, relationships, indexes, and constraints.

### 4. Build the Backend

```bash
dotnet build
```

### 5. Run the Backend

```bash
dotnet run --project backend/TrackManagement.Api
```

The API has been tested locally at:

`http://localhost:5150`

The actual port may vary depending on the launch configuration.

### 6. Install Frontend Dependencies

Open another terminal at the repository root:

```bash
cd frontend
npm install
```

### 7. Run the React Frontend

```bash
npm run dev
```

Open:

`http://localhost:5173`

Both the backend and frontend should be running for the React application to retrieve tracks from the database.

### 8. Build the Frontend

From the `frontend` directory:

```bash
npm run build
```

This runs TypeScript checks and generates the production frontend build.

The frontend production build has been tested successfully.

### CORS Configuration

The backend includes a CORS policy allowing requests from the React development server at:

`http://localhost:5173`

The policy is configured in `backend/TrackManagement.Api/Program.cs`.

If the frontend uses a different port or origin, the allowed origin must be updated accordingly.

## Database Seeding

In the Development environment, the application automatically seeds an initially empty database with:

- 3 artists
- 8 tracks across different genres and statuses
- 3 DSPs: Spotify, Apple Music, and YouTube
- 7 initial track distribution records

The seeding logic checks for existing data to avoid repeatedly adding the initial sample records during normal restarts.

Additional records may exist after API testing.

## Domain Model

The system contains four main entities:

- **Artist:** Stores artist information such as name, email, and country.
- **Track:** Stores track information, including title, artist, ISRC, release date, genre, and overall status.
- **DSP:** Represents a digital music platform.
- **TrackDistribution:** Connects a track to a DSP and stores its submission date and platform-specific status.

An artist can own multiple tracks. A track can be submitted to multiple DSPs, and each DSP can receive multiple tracks.

### Status Types

**TrackStatus** represents a track's overall workflow:

- `draft`
- `submitted`
- `distributed`

**DistributionStatus** represents the status of a specific track on a specific DSP:

- `pending`
- `live`
- `rejected`

New tracks begin as `draft`. When a draft is submitted to one or more DSPs, its overall status changes to `submitted`, and each new distribution record begins as `pending`.

The overall track status can also be updated through the PATCH status endpoint.

## Implemented API Endpoints

All seven required API endpoints have been implemented and tested using Postman.

| Method | Endpoint | Description | Status |
|---|---|---|---|
| POST | `/api/artists` | Create a new artist | Completed |
| GET | `/api/artists` | Retrieve all artists | Completed |
| POST | `/api/tracks` | Create a track for an existing artist | Completed |
| GET | `/api/tracks` | List tracks with optional filters | Completed |
| GET | `/api/tracks/{id}` | Retrieve track details and DSP distribution statuses | Completed |
| PATCH | `/api/tracks/{id}/status` | Update a track's overall status | Completed |
| POST | `/api/tracks/{id}/distribute` | Submit a track to one or more DSPs | Completed — JWT protected |

### Track Filtering

The Track List endpoint supports filtering by:

- `artistId`
- `genre`
- `status`

Example:

```http
GET /api/tracks?genre=Pop&status=distributed
```

### Updating Track Status

```http
PATCH /api/tracks/3/status
Content-Type: application/json
```

```json
{
  "status": "submitted"
}
```

### Distributing a Track

```http
POST /api/tracks/3/distribute
Authorization: Bearer <your-development-token>
Content-Type: application/json
```

```json
{
  "dspIds": [1, 2]
}
```

The supplied DSP IDs must exist in the database, and duplicate track–DSP combinations are rejected.

## Input Validation and Data Integrity

The API includes:

- Required-field validation
- Maximum string-length validation
- Email format validation
- ISRC format validation
- Artist existence checks
- Duplicate ISRC checks
- Track existence checks
- Track status validation
- DSP existence checks
- Duplicate DSP ID validation
- Duplicate track distribution checks

Database-level unique indexes also enforce ISRC uniqueness and unique track–DSP combinations.

## JWT Authentication

The application uses JWT Bearer authentication to protect the track distribution endpoint.

**Protected endpoint:**

`POST /api/tracks/{id}/distribute`

### Generate a Development JWT

From the repository root, run:

```powershell
dotnet user-jwts create --project backend/TrackManagement.Api --name LocalTester --valid-for 1d
```

This generates a development JWT for local API testing.

The development signing key is stored locally using .NET development tooling and must not be committed to GitHub.

### Using JWT in Postman

1. Open the track distribution POST request.
2. Select **Authorization → Bearer Token**.
3. Paste the generated JWT into the Token field.
4. Send the request with the selected DSP IDs.

Example:

```json
{
  "dspIds": [1, 2]
}
```

### Authentication and Validation Testing

The following scenarios were verified in Postman:

| Scenario | HTTP Status |
|---|---|
| Valid JWT and valid distribution request | `200 OK` |
| Missing JWT | `401 Unauthorized` |
| Duplicate distribution | `409 Conflict` |
| Invalid DSP ID | `400 Bad Request` |

Development JWTs are for local testing only. A production deployment would require a proper authentication and identity-management approach.

## Frontend

The frontend is built using React, TypeScript, and Vite.

### Track List — Implemented

The Track List dashboard currently provides:

- Tracks retrieved from the ASP.NET Core API
- Track titles and artist names
- Genres and overall statuses
- ISRC and release dates
- Status filtering: All, Draft, Submitted, Distributed
- Search by track title, artist, or genre
- Summary cards showing counts by status
- Responsive dashboard styling
- Loading and error states

The React application fetches data from:

`GET /api/tracks`

The search and status filter operate on the tracks already loaded into React state.

### Track Detail — Pending

The next frontend screen will display:

- Selected track information
- Related DSP distributions
- Individual distribution statuses
- Submission dates where available
- Navigation back to the Track List

It will use:

`GET /api/tracks/{id}`

## Testing

### Backend

The implemented endpoints were tested using Postman, including:

- Creating artists and tracks
- Retrieving artists and tracks
- Filtering tracks
- Retrieving track details and distributions
- Updating track statuses
- Distributing tracks to selected DSPs
- Rejecting missing JWTs
- Rejecting invalid DSP IDs
- Rejecting duplicate distributions
- Rejecting invalid artist email input

### Frontend

The React Track List has been tested in the browser with data returned from the running backend.

The status dropdown and search field update the displayed tracks without a page reload.

The frontend production build has also completed successfully.

### Remaining Testing

- Track Detail integration testing
- Additional edge-case and security review
- Improved handling of concurrent duplicate database operations
- Final full-stack verification

## Implementation Progress

- [x] Clean Architecture solution setup
- [x] Domain entities and enums
- [x] SQL Server integration with EF Core
- [x] Initial database migration
- [x] Development database seeding
- [x] All seven required REST API endpoints
- [x] JWT-protected distribution endpoint
- [x] Postman API testing
- [x] React + TypeScript + Vite setup
- [x] CORS configuration
- [x] React Track List
- [x] Status filtering and track search
- [x] Dashboard styling
- [x] Frontend production build
- [ ] React Track Detail page
- [ ] Final integration and security review
- [ ] Final documentation review and submission

## Development Decisions

See [DECISIONS.md](DECISIONS.md) for information about AI-assisted development, implementation decisions, security considerations, and errors identified and corrected during development.