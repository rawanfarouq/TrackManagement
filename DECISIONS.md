# Development Decisions & AI Usage

## 1. What did AI generate for you, and what did you write or modify yourself?

I used ChatGPT throughout development as an assistant for planning, code generation, technical explanations, and debugging.

I chose React and SQL Server based on my previous experience with these technologies and the limited assessment deadline.

### Backend Architecture

AI helped plan the Clean Architecture structure and generate the initial .NET CLI commands.

I executed the setup commands, configured project references, installed the required tools, and verified the builds.

The backend was divided into Domain, Application, Infrastructure, and API projects.

### Domain and Infrastructure

AI assisted with generating the Artist, Track, DSP, and TrackDistribution entities, status enums, EF Core configuration, repository implementations, migrations, and database seeding.

I reviewed the entity relationships, foreign keys, navigation properties, and the difference between overall track status and DSP distribution status.

I executed the migration commands and verified the database schema and seeded records using SQL Server Management Studio.

### Application and API

AI assisted with generating DTOs, interfaces, services, controllers, and business logic.

This included track creation, filtering, status updates, DSP distribution, validation, and JWT authentication.

I integrated the code, configured Dependency Injection, reviewed runtime errors, and tested the endpoints in Postman.

I verified all seven required endpoints, including successful requests and error responses.

### React Frontend

AI assisted with creating the React + TypeScript + Vite application and generating the frontend components, API service functions, TypeScript interfaces, state management, filtering, and CSS.

I integrated and tested these changes in the browser.

The frontend includes a Track List dashboard with search and status filtering, and a Track Detail view displaying individual track information and DSP distribution records.

During implementation, I noticed that the initial Track Detail styling did not display correctly. I addressed this by using a separate `TrackDetail.css` stylesheet and refining the layout.

I also integrated CORS configuration so the frontend could communicate with the ASP.NET Core backend.

### Testing, Git, and Documentation

I tested the backend using Postman and the frontend using browser interactions.

I executed the final .NET build, React production build, and ESLint checks. All completed successfully.

I also performed browser acceptance checks covering filtering, searching, details, DSP statuses, empty states, navigation, and console errors.

I used Git and GitHub to maintain separate commits for development milestones rather than creating one large commit.

AI helped draft README.md and DECISIONS.md, which I reviewed and updated to reflect the implementation.

AI was used as a development assistant; I remained responsible for integrating, running, reviewing, and testing the application.

## 2. What security issues did you find (or introduce) in the AI-generated code? How did you handle them?

### Authentication and Authorization

The distribution endpoint changes database records and should not be anonymously accessible.

I protected `POST /api/tracks/{id}/distribute` using JWT Bearer authentication and `[Authorize]`.

I verified through Postman that a valid development JWT permitted the request while a missing JWT returned `401 Unauthorized`.

### Development Tokens and Secrets

I used the .NET development JWT tooling instead of hardcoding signing secrets into the source code.

The Git ignore configuration excludes local development authentication settings and environment files.

This development-token approach is suitable for demonstrating an authenticated endpoint but is not production-ready user authentication.

### Input Validation

The implementation uses Data Annotations and application-level validation for required fields, email formats, ISRC values, statuses, artist IDs, and DSP IDs.

I tested invalid inputs through Postman to check that meaningful error responses were returned.

### Duplicate Prevention and Data Integrity

The application checks for existing ISRC values and existing track–DSP distribution records.

Database-level unique constraints provide additional protection against duplicates.

I tested duplicate distribution submissions and confirmed that the API returned `409 Conflict`.

### Local Database Configuration

The development environment uses SQL Server Express with Windows Authentication.

`TrustServerCertificate=True` is used for the local database connection, but it should not be considered a production security setting.

### CORS

The API permits browser requests from the local React development origin, `http://localhost:5173`, rather than using an unrestricted wildcard origin.

CORS is not a replacement for authentication or authorization.

## 3. One thing the AI got wrong that you had to fix. What was it and why was it wrong?

One error in the AI-assisted implementation was the omission of Dependency Injection registrations for the Track service and repository in `Program.cs`.

The backend compiled successfully, but a Track API request failed at runtime with `500 Internal Server Error`.

The application log showed:

`Unable to resolve service for type 'ITrackService' while attempting to activate 'TracksController'.`

I inspected `Program.cs` and identified that `ITrackService` and `ITrackRepository` had not been registered with the dependency injection container.

I fixed the error by adding:

```csharp
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<ITrackService, TrackService>();
```

I placed these registrations before `builder.Build()`, rebuilt the application, restarted the backend, and tested the Track endpoints in Postman.

The endpoints then worked correctly.

The issue occurred because ASP.NET Core requires the concrete service implementations to be registered before they can be injected into controllers and other services.

This experience demonstrated that AI-generated code can be incomplete and that successful compilation does not guarantee correct runtime behavior. Inspecting application logs and testing the API helped me find and correct the problem.

