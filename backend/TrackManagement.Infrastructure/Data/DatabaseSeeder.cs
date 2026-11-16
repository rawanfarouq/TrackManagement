
using Microsoft.EntityFrameworkCore;
using TrackManagement.Domain.Entities;
using TrackManagement.Domain.Enums;

namespace TrackManagement.Infrastructure.Data;

public static class DatabaseSeeder
{
    //method that will populate our database.
    public static async Task SeedAsync(TrackManagementDbContext db)
    {
        // Avoid inserting duplicate sample data
        //checks whether records already exist
        if (await db.Artists.AnyAsync() ||
            await db.Tracks.AnyAsync() ||
            await db.DSPs.AnyAsync())
        {
            return;
        }

        // Create artists
        var artists = new[]
        {
            new Artist { Name = "Layla Nasser", Email = "layla@example.com", Country = "Egypt" },
            new Artist { Name = "Omar Haddad", Email = "omar@example.com", Country = "Morocco" },
            new Artist { Name = "Maya Khalil", Email = "maya@example.com", Country = "Lebanon" }
        };

       //used to add multiple objects to the database in one operation
        db.Artists.AddRange(artists);

        // Create distribution platforms
        var dsps = new[]
        {
            new DSP { Name = "Spotify" },
            new DSP { Name = "Apple Music" },
            new DSP { Name = "YouTube" }
        };

        db.DSPs.AddRange(dsps);

        //actually saves them into SQL Server
        await db.SaveChangesAsync();

        // Create tracks
        var tracks = new[]
        {
            new Track { Title = "Nile Lights", ArtistId = artists[0].Id, Isrc = "EGABC2600001", ReleaseDate = new DateOnly(2026, 2, 15), Genre = "Pop", Status = TrackStatus.Distributed },
            new Track { Title = "Cairo Sunrise", ArtistId = artists[0].Id, Isrc = "EGABC2600002", ReleaseDate = new DateOnly(2026, 5, 10), Genre = "Electronic", Status = TrackStatus.Submitted },
            new Track { Title = "Desert Echoes", ArtistId = artists[0].Id, Isrc = "EGABC2600003", ReleaseDate = new DateOnly(2026, 12, 1), Genre = "Ambient", Status = TrackStatus.Draft },

            new Track { Title = "Atlas Beat", ArtistId = artists[1].Id, Isrc = "MAABC2600004", ReleaseDate = new DateOnly(2026, 3, 20), Genre = "Hip-Hop", Status = TrackStatus.Distributed },
            new Track { Title = "Blue Medina", ArtistId = artists[1].Id, Isrc = "MAABC2600005", ReleaseDate = new DateOnly(2026, 7, 12), Genre = "Jazz", Status = TrackStatus.Submitted },
            new Track { Title = "Night Ride", ArtistId = artists[1].Id, Isrc = "MAABC2600006", ReleaseDate = new DateOnly(2027, 1, 5), Genre = "Rock", Status = TrackStatus.Draft },

            new Track { Title = "Cedar Skies", ArtistId = artists[2].Id, Isrc = "LBABC2600007", ReleaseDate = new DateOnly(2026, 4, 22), Genre = "Indie Pop", Status = TrackStatus.Distributed },
            new Track { Title = "Beirut Rain", ArtistId = artists[2].Id, Isrc = "LBABC2600008", ReleaseDate = new DateOnly(2026, 8, 18), Genre = "Acoustic", Status = TrackStatus.Submitted }
        };

        db.Tracks.AddRange(tracks);
        await db.SaveChangesAsync();

        // Add example distribution records
        db.TrackDistributions.AddRange(
            new TrackDistribution { TrackId = tracks[0].Id, DspId = dsps[0].Id, SubmittedAt = DateTime.UtcNow.AddDays(-10), Status = DistributionStatus.Live },
            new TrackDistribution { TrackId = tracks[0].Id, DspId = dsps[1].Id, SubmittedAt = DateTime.UtcNow.AddDays(-8), Status = DistributionStatus.Live },
            new TrackDistribution { TrackId = tracks[1].Id, DspId = dsps[0].Id, SubmittedAt = DateTime.UtcNow.AddDays(-2), Status = DistributionStatus.Pending },
            new TrackDistribution { TrackId = tracks[3].Id, DspId = dsps[2].Id, SubmittedAt = DateTime.UtcNow.AddDays(-7), Status = DistributionStatus.Live },
            new TrackDistribution { TrackId = tracks[4].Id, DspId = dsps[1].Id, SubmittedAt = DateTime.UtcNow.AddDays(-3), Status = DistributionStatus.Pending },
            new TrackDistribution { TrackId = tracks[6].Id, DspId = dsps[0].Id, SubmittedAt = DateTime.UtcNow.AddDays(-5), Status = DistributionStatus.Live },
            new TrackDistribution { TrackId = tracks[7].Id, DspId = dsps[2].Id, SubmittedAt = DateTime.UtcNow.AddDays(-1), Status = DistributionStatus.Rejected }
        );

        await db.SaveChangesAsync();
    }
}
