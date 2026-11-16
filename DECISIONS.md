# Development Decisions & AI Usage

## 1. What did AI generate, and what did I write or modify myself?

AI assistance was used to understand the assignment requirements, plan the project structure, and generate the initial .NET CLI commands for setting up the solution using Clean Architecture.

I chose **React and SQL Server** based on my previous experience with these technologies and the 72-hour submission deadline, allowing me to focus on implementation rather than learning a new stack.

I executed the setup commands, configured the solution and project references, installed the necessary development tools, and verified that the backend projects built successfully.

For the Domain layer, AI helped generate the initial entity classes (`Artist`, `Track`, `DSP`, and `TrackDistribution`) and status enums. I reviewed their properties, relationships, and navigation properties, including the purpose of ISRC and `ICollection<T>`, to understand how the entities relate to each other.

AI also assisted with implementing the Infrastructure layer, including the EF Core DbContext, entity relationships, unique constraints, SQL Server configuration, migrations, and database seeding.

I executed the migration commands and verified the resulting database schema and sample data using SQL Server Management Studio. I also reviewed the purpose of DbContext, DbSet, migrations, AddRange, and SaveChangesAsync to understand the implementation.

For the Application and API layers, AI assisted in generating the Artist and Track DTOs, repository interfaces, repository implementations, services, and controllers.

I reviewed the generated code, configured the required dependencies, resolved runtime issues, and tested the implemented API endpoints using Postman, including creating records, retrieving data, filtering tracks, and checking input validation.

I also initialized the Git repository and maintained separate commits for major development milestones.

AI assisted with implementing the track status update, distribution service, JWT authentication configuration, and the protected distribution endpoint.

I tested all seven required API endpoints using Postman, including successful requests and error scenarios.

For the protected endpoint, I verified that requests without a JWT returned `401 Unauthorized`, duplicate distribution requests returned `409 Conflict`, and invalid DSP IDs returned `400 Bad Request`.

I also used the .NET development JWT tool to generate local test tokens rather than hardcoding authentication secrets in the repository.

This section will continue to be updated as implementation progresses, distinguishing AI-generated code from my own modifications.

## 2. What security issues did I identify in AI-generated code, and how did I handle them?

During development, I reviewed potential risks related to input validation, database integrity, and local configuration.

The implementation currently includes Data Annotation validation for API requests and database-level unique indexes to prevent duplicate ISRC values and track/DSP combinations.

I also used Windows Authentication for the local SQL Server connection. The configuration includes `TrustServerCertificate=True` for local development, which should not be used as a general production security configuration.

During the security review, I checked whether the track distribution endpoint could be accessed without authentication. I protected the endpoint using JWT Bearer authentication and the `[Authorize]` attribute, then verified that unauthenticated requests were rejected.

I also tested input validation and duplicate prevention. The application checks for invalid DSP IDs and existing distribution records, while database-level unique constraints provide additional protection against duplicate entries.

JWT development credentials are kept outside the source-controlled application configuration. The current development-token approach is intended for local testing and is not a replacement for production authentication.


## 3. What did AI get wrong that I had to fix, and why?

One issue I encountered in the AI-assisted implementation was that the required Dependency Injection registrations for the Track service and repository were missing from `Program.cs`.

The project built successfully, but when I ran the application and tested the Track API using Postman, the request failed with a `500 Internal Server Error`.

After inspecting the terminal logs, I identified the error:

`Unable to resolve service for type 'ITrackService' while attempting to activate 'TracksController'.`

I reviewed `Program.cs` and found that the `ITrackService` and `ITrackRepository` implementations had not been registered with the Dependency Injection container.

I corrected the issue by adding the missing registrations before `builder.Build()`:

```csharp
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<ITrackService, TrackService>();
```

After rebuilding and restarting the application, I tested the Track endpoints again using Postman, and they worked successfully.

This experience showed me that AI-generated implementations should not be assumed to be complete, and that successful compilation alone does not guarantee correct runtime behavior. Reviewing error logs and testing the actual endpoints helped me identify and resolve the problem.