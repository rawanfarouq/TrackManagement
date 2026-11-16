
using Microsoft.EntityFrameworkCore;
using TrackManagement.Infrastructure.Data;
using TrackManagement.Application.Interfaces;
using TrackManagement.Application.Services;
using TrackManagement.Infrastructure.Repositories;

// 1. Create the application builder
var builder = WebApplication.CreateBuilder(args);

// 2. Register services (Dependency Injection)

// Register SQL Server DbContext
builder.Services.AddDbContext<TrackManagementDbContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "Database connection string not found."
        )
    )
);

// Register repositories
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();

// Register application services
builder.Services.AddScoped<IArtistService, ArtistService>();

// Track registrations
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<ITrackService, TrackService>();


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer();

builder.Services.AddAuthorization();



// Register controllers and OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

//React frontend 
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// 3. Build the application AFTER registering services
var app = builder.Build();

// 4. Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Seed database with sample data
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<TrackManagementDbContext>();

    await DatabaseSeeder.SeedAsync(db);
}



app.UseHttpsRedirection();

app.UseCors("ReactFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();



// 5. Start the application
app.Run();
