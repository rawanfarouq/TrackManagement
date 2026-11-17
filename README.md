# Track Management System

A full-stack music track management application developed for Takwene's Full-Stack Software Engineer technical assessment.

The system manages artists, tracks, and music distribution to Digital Service Providers (DSPs), including Spotify, Apple Music, and YouTube.

## Tech Stack

- **Backend:** ASP.NET Core Web API (.NET 10)
- **Frontend:** React, TypeScript, Vite, CSS
- **Database:** Microsoft SQL Server 2022 Express
- **ORM:** Entity Framework Core 10
- **Authentication:** JWT Bearer
- **API Testing:** Postman
- **Version Control:** Git and GitHub

## Architecture

The backend follows Clean Architecture and consists of four projects:

- **Domain:** Business entities, relationships, and status enums
- **Application:** DTOs, interfaces, services, validation, and business logic
- **Infrastructure:** EF Core, SQL Server access, repositories, migrations, and seeding
- **API:** REST controllers, dependency injection, JWT authentication, and CORS

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
│   │   ├── components/
│   │   │   ├── TrackDetail.tsx
│   │   │   └── TrackDetail.css
│   │   ├── services/
│   │   │   └── trackApi.ts
│   │   ├── types/
│   │   │   └── track.ts
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
- Git
- SQL Server Management Studio (optional)
- Postman (optional)

### 1. Configure the Database

The application was developed with the following local configuration:

- Server: `localhost\SQLEXPRESS`
- Database: `TrackManagementDb`
- Authentication: Windows Authentication

Update `DefaultConnection` in `backend/TrackManagement.Api/appsettings.json` if your SQL Server configuration differs.

`TrustServerCertificate=True` is used for the local development environment and is not a recommended production default.

### 2. Restore Backend Dependencies

From the repository root:

```powershell
dotnet restore
dotnet tool restore
```

### 3. Apply Database Migrations

```powershell
dotnet ef database update --project backend/TrackManagement.Infrastructure --startup-project backend/TrackManagement.Api --context TrackManagementDbContext
```

This applies the existing EF Core migrations and creates the required tables and constraints.

### 4. Build and Run the Backend

```powershell
dotnet build
dotnet run --project backend/TrackManagement.Api
```

The backend has been tested locally at:

`http://localhost:5150`

The actual port may depend on the local launch configuration.

### 5. Install and Run the Frontend

Open a second terminal at the repository root:

```powershell
cd frontend
npm install
npm run dev
```

Open:

`http://localhost:5173`

The backend must also be running so the frontend can retrieve track data.

The frontend API service currently uses `http://localhost:5150` as its backend base URL. Update it if the API runs at a different address.

### 6. Build and Lint the Frontend

From the repository root:

```powershell
npm --prefix frontend run build
npm --prefix frontend run lint
```

Both commands have been executed successfully during final verification.

### CORS

The backend permits requests from the local React development origin:

`http://localhost:5173`

The policy is configured in `backend/TrackManagement.Api/Program.cs`.

## Database Seeding

In the Development environment, an initially empty database is seeded with:

- 3 artists
- 8 tracks with different genres and statuses
- 3 DSPs: Spotify, Apple Music, and YouTube
- 7 initial distribution records

The application checks for existing data to avoid repeating the initial seed during normal restarts.

Additional tracks and distributions may exist after API testing.

## Domain Model

The application uses four main entities:

| Entity | Description |
|---|---|
| Artist | Artist name, email, country, and associated tracks |
| Track | Title, artist, ISRC, release date, genre, and overall status |
| DSP | Digital music distribution platform |
| TrackDistribution | Track-to-DSP relationship, submission date, and individual status |

An artist can have multiple tracks. A track can be submitted to multiple DSPs, and a DSP can receive multiple tracks.

### Status Types

**TrackStatus** represents the overall track workflow:

- `draft`
- `submitted`
- `distributed`

**DistributionStatus** represents a track's status on a specific DSP:

- `pending`
- `live`
- `rejected`

New tracks start as `draft`. Distributing a draft track changes its overall status to `submitted`, while new distribution records start as `pending`.

The overall track status can also be updated through the PATCH endpoint.

## API Endpoints

All seven required endpoints have been implemented.

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/artists` | Create an artist |
| GET | `/api/artists` | Retrieve all artists |
| POST | `/api/tracks` | Create a track |
| GET | `/api/tracks` | Retrieve and filter tracks |
| GET | `/api/tracks/{id}` | Retrieve track details and DSP distributions |
| POST | `/api/tracks/{id}/distribute` | Distribute a track to selected DSPs — JWT protected |
| PATCH | `/api/tracks/{id}/status` | Update the overall track status |


### Update Track Status

```http
PATCH /api/tracks/3/status
Content-Type: application/json
```

```json
{
  "status": "submitted"
}
```

### Distribute a Track

```http
POST /api/tracks/3/distribute
Authorization: Bearer <development-token>
Content-Type: application/json
```

```json
{
  "dspIds": [1, 2]
}
```

All selected DSP IDs must exist. Duplicate DSP IDs and existing track–DSP combinations are rejected.

## Input Validation and Database Integrity

The API provides validation for required fields, email formats, ISRC formats, field lengths, track statuses, artist existence, DSP existence, and duplicate distribution submissions.

Unique database indexes enforce ISRC uniqueness and prevent duplicate track–DSP combinations.

## JWT Authentication

The track distribution endpoint uses JWT Bearer authentication with `[Authorize]`.

### Obtain a Development JWT

From the repository root:

```powershell
dotnet user-jwts create --project backend/TrackManagement.Api --name LocalTester --valid-for 1d
```

This generates a JWT for local development testing.

In Postman, open the distribution request, select **Authorization → Bearer Token**, and paste the generated token.

Development JWT signing keys and tokens must not be committed to GitHub.

This development-token approach demonstrates authentication but is not a production identity-management system.

## Frontend

The frontend uses React, TypeScript, and Vite. Both required views are implemented.

### Track List

The Track List dashboard displays:

- Track title and artist name
- Genre and overall status
- ISRC and release date
- Summary cards showing track counts
- Status filtering
- Search by track title, artist, or genre
- Loading, error, and empty-result states

The frontend retrieves track data using:

`GET /api/tracks`

Filtering and searching operate on the tracks loaded into React state.

### Track Detail

Clicking a track title opens its detail view.

The Track Detail page displays:

- Title and artist
- Genre and overall status
- ISRC and release date
- DSP distribution records
- Individual DSP statuses
- Submission dates
- A message when no distributions exist
- Back navigation to the Track List

Track details are fetched using:

`GET /api/tracks/{id}`

The application currently uses React state to switch between the two views rather than URL-based routing.

### Styling

The interface uses a responsive dashboard layout with status badges, summary cards, tables, and detail cards.

The Track Detail component has its own stylesheet, `TrackDetail.css`.

## Testing and Verification

### Backend

The required endpoints were tested in Postman, including:

- Artist creation and retrieval
- Track creation, retrieval, and filtering
- Individual track details
- Track status updates
- Authenticated distribution submissions
- Invalid artist email validation
- Invalid DSP IDs
- Duplicate distributions
- Missing JWT authentication

Selected verified responses:

| Scenario | Expected and observed result |
|---|---|
| Valid distribution request with JWT | `200 OK` |
| Missing JWT | `401 Unauthorized` |
| Duplicate distribution | `409 Conflict` |
| Invalid DSP | `400 Bad Request` |

### Frontend

Final browser checks passed for:

- Loading the Track List
- Filtering by all status options
- Searching tracks by title, artist, and genre
- Opening the selected Track Detail page
- Displaying correct DSP distributions and submission dates
- Displaying the empty state for tracks without distributions
- Returning to the Track List
- No browser console errors during the tested interactions

### Build Verification

The following commands completed successfully:

```powershell
dotnet tool restore
dotnet build
npm --prefix frontend run build
npm --prefix frontend run lint
```


## Development Decisions

See [DECISIONS.md](DECISIONS.md) for AI-assisted development, security considerations, technical decisions, and errors identified and corrected during implementation.
