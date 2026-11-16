# Development Decisions & AI Usage

## 1. What did AI generate, and what did I write or modify myself?

AI assistance was used throughout development to understand the assignment requirements, plan the implementation, generate initial code, explain technical concepts, and troubleshoot issues.

I chose **React and SQL Server** based on my previous experience with these technologies and the 72-hour submission deadline. This allowed me to focus on completing the required application rather than learning an entirely unfamiliar technology stack.

### Backend Setup and Architecture

AI helped plan the Clean Architecture structure and generate the initial .NET CLI commands for creating the solution.

I executed the commands, configured project references, installed the required development tools, and verified that the four backend projects built successfully.

The backend was organized into Domain, Application, Infrastructure, and API projects to separate business entities, application logic, database access, and HTTP request handling.

### Domain Layer

AI assisted with generating the `Artist`, `Track`, `DSP`, and `TrackDistribution` entities and their status enums.

I reviewed the properties, foreign keys, navigation properties, and relationships. I also examined the roles of ISRC, primary keys, foreign keys, `ICollection<T>`, and the junction entity used to represent track distribution to different DSPs.

### Infrastructure Layer

AI assisted with implementing the Entity Framework Core DbContext, database configuration, relationships, constraints, migrations, repositories, and sample-data seeding.

I executed the migration commands and verified the resulting schema and seeded records in SQL Server Management Studio.

I also reviewed how DbContext, DbSet, EF Core change tracking, `AddAsync`, `SaveChangesAsync`, `Include`, and `AsNoTracking` work.

### Application and API Layers

AI helped generate the Artist and Track DTOs, service and repository interfaces, service implementations, repository implementations, and controllers.

I integrated the generated code, checked compilation and runtime behavior, reviewed error messages, and tested the API endpoints using Postman.

AI also assisted with:

- Track creation, listing, and filtering
- Track status updates
- Track distribution logic
- DSP validation and duplicate-prevention checks
- JWT authentication configuration
- The protected distribution endpoint

I verified all seven required API endpoints, including successful requests and relevant error cases.

In particular, I confirmed that missing JWT authentication returned `401 Unauthorized`, duplicate distribution requests returned `409 Conflict`, and invalid DSP IDs returned `400 Bad Request`.

I generated development test tokens using the .NET JWT development tool instead of hardcoding signing secrets into the application source code.

### React Frontend

AI assisted with setting up React, TypeScript, and Vite, organizing the frontend files, connecting the frontend to the backend API, and implementing the Track List dashboard.

I executed the frontend setup commands, selected ESLint, installed dependencies, and verified that the Vite development server started successfully.

AI helped generate the initial React components, TypeScript interfaces, API-fetching function, status filtering, search functionality, and CSS dashboard styling.

I integrated these changes into the project and tested the interface in the browser using real data returned by the ASP.NET Core backend.

I verified that the Track List displayed data, that filtering changed the displayed results, and that the React production build completed successfully.

AI also assisted with configuring CORS so the frontend running at `http://localhost:5173` could communicate with the backend running at `http://localhost:5150`.

### Version Control and Documentation

I initialized the Git repository, maintained commits for development milestones, and pushed the implementation to GitHub.

AI helped prepare and revise the README and development decisions documentation. I reviewed and updated the documents to reflect the actual implementation and testing progress.

Throughout the project, I used AI as a development assistant while remaining responsible for running the code, verifying functionality, understanding the implementation, and identifying issues.

## 2. What security issues did I identify in AI-generated code, and how did I handle them?

During development, I reviewed potential security and reliability risks related to authentication, input validation, database integrity, and configuration.

### Authentication and Authorization

The track distribution endpoint changes application data, so it should not be publicly accessible without authentication.

I configured JWT Bearer authentication and protected the distribution endpoint with the `[Authorize]` attribute.

I tested the endpoint in Postman and verified that requests without a JWT returned `401 Unauthorized`, while requests with a valid development JWT could proceed.

The current authentication approach uses locally generated development tokens. It is suitable for demonstrating a protected API endpoint but is not a complete production authentication or user-management solution.

### Secret Management

I used the .NET development JWT tooling rather than hardcoding a signing key into the repository.

Development tokens and signing keys should remain outside source-controlled files.

A production system should use secure secret storage and a proper authentication provider or identity-management solution.

### Input Validation

The API uses Data Annotations and application-level checks for request validation.

Examples include required fields, valid email formats, ISRC validation, valid status values, and checking that related Artist and DSP records exist.

These checks help prevent invalid data from entering the application.

### Database Integrity

The database includes unique constraints for ISRC values and track–DSP combinations.

The distribution service also checks whether a track has already been submitted to a selected DSP before creating new records.

I tested duplicate distribution attempts and verified that the API returned `409 Conflict`.

A remaining improvement is handling simultaneous requests that violate a database unique constraint, ensuring they return a controlled error response instead of an unhandled server error.

### Local Database Configuration

The application uses Windows Authentication with SQL Server Express for local development.

The connection string includes `TrustServerCertificate=True`, which is convenient for the local development database but should not be treated as a secure default for production environments.

### CORS

AI assisted with configuring a CORS policy for the React frontend.

Instead of allowing requests from every origin, the policy explicitly allows the local React development origin, `http://localhost:5173`.

CORS helps control browser-based cross-origin access; it is not a replacement for authentication or authorization.

### Remaining Security Considerations

Before a production deployment, the application would benefit from stronger identity management, secure deployment configuration, automated security testing, and more complete handling of database concurrency and unexpected failures.

The scope of the current implementation is a technical assessment and local demonstration.

## 3. What did AI get wrong that I had to fix, and why?

One issue I encountered in the AI-assisted implementation was that the required Dependency Injection registrations for the Track service and repository were missing from `Program.cs`.

The project built successfully, but when I ran the application and tested the Track API in Postman, the request failed with a `500 Internal Server Error`.

After checking the terminal logs, I identified the error:

`Unable to resolve service for type 'ITrackService' while attempting to activate 'TracksController'.`

I inspected `Program.cs` and discovered that `ITrackService` and `ITrackRepository` had not been registered with the Dependency Injection container.

I fixed the issue by adding:

```csharp
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<ITrackService, TrackService>();
```

I placed these registrations before `builder.Build()`, rebuilt the application, and restarted the API.

I then tested the Track endpoints again using Postman, and they worked successfully.

This showed me that **successful compilation does not guarantee correct runtime behavior**, and that AI-generated code still requires careful review and testing.

It also reinforced the importance of understanding Dependency Injection, checking application logs, and validating features through actual API requests instead of relying only on generated code.

## 4. Additional Design Decisions

### Why Clean Architecture?

I used Clean Architecture to separate responsibilities and improve maintainability. Services contain application logic, while repositories implement database operations through EF Core.

This also makes the application easier to extend or test without tightly coupling the business logic to SQL Server.

### Why React and TypeScript?

I chose React because of my prior experience with it and the limited assessment timeframe.

TypeScript provides stronger typing for API responses and component state, helping reduce errors when working with track and distribution data.

### Why SQL Server and EF Core?

I had previous experience with SQL Server, and EF Core integrates naturally with ASP.NET Core.

EF Core migrations also make the database schema reproducible from the source code.

### Why Separate Track and Distribution Statuses?

A track has one overall workflow status, but it can have different distribution statuses on different DSPs.

Separating `TrackStatus` from `DistributionStatus` allows the application to represent these two concepts independently.

### Why Use Frontend Filtering?

The current Track List loads the catalog through the backend API and performs search and status filtering on the retrieved data.

This approach is simple and appropriate for the small dataset used in the assessment.

For a larger production catalog, server-side search, filtering, and pagination would be preferable.

### Future Improvements

If more development time were available, I would prioritize automated unit and integration tests, production-ready authentication, stronger concurrency handling, pagination, and additional frontend workflows.