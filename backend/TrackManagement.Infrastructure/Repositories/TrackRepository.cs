
using Microsoft.EntityFrameworkCore;
using TrackManagement.Application.Interfaces;
using TrackManagement.Domain.Entities;
using TrackManagement.Domain.Enums;
using TrackManagement.Infrastructure.Data;

namespace TrackManagement.Infrastructure.Repositories;

public class TrackRepository : ITrackRepository
{
    private readonly TrackManagementDbContext _db;

    public TrackRepository(TrackManagementDbContext db)
    {
        _db = db;
    }

    // Create a track
    public async Task AddAsync(
        Track track,
        CancellationToken cancellationToken = default)
    {
        await _db.Tracks.AddAsync(track, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    // Retrieve and filter tracks
    public async Task<IReadOnlyList<Track>> GetAllAsync(
        int? artistId,
        string? genre,
        TrackStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Tracks
            .AsNoTracking()
            .Include(t => t.Artist)
            .AsQueryable();

        if (artistId.HasValue)
        {
            query = query.Where(t =>
                t.ArtistId == artistId.Value);
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(t =>
                t.Genre == genre.Trim());
        }

        if (status.HasValue)
        {
            query = query.Where(t =>
                t.Status == status.Value);
        }

        return await query
            .OrderBy(t => t.Id)
            .ToListAsync(cancellationToken);
    }

    // Retrieve a track and its DSP distributions
    public async Task<Track?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _db.Tracks
            .AsNoTracking()
            .Include(t => t.Artist)
            .Include(t => t.TrackDistributions)
                .ThenInclude(td => td.Dsp)
            .FirstOrDefaultAsync(
                t => t.Id == id,
                cancellationToken);
    }

    // Check if artist exists
    public async Task<bool> ArtistExistsAsync(
        int artistId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Artists.AnyAsync(
            a => a.Id == artistId,
            cancellationToken);
    }

    // Check if ISRC exists
    public async Task<bool> IsrcExistsAsync(
        string isrc,
        CancellationToken cancellationToken = default)
    {
        return await _db.Tracks.AnyAsync(
            t => t.Isrc == isrc,
            cancellationToken);
    }


    public async Task<Track?> UpdateStatusAsync(
        int id,
        TrackStatus status,
        CancellationToken cancellationToken = default)
    {
        var track = await _db.Tracks
            .Include(t => t.Artist)
            .FirstOrDefaultAsync(
                t => t.Id == id,
                cancellationToken);

        if (track is null)
        {
            return null;
        }

        track.Status = status;

        await _db.SaveChangesAsync(cancellationToken);

        return track;
    }


    // Get a track for modification, including its distributions
    public async Task<Track?> GetForDistributionAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _db.Tracks
            .Include(t => t.Artist)
            .Include(t => t.TrackDistributions)
            .FirstOrDefaultAsync(
                t => t.Id == id,
                cancellationToken);
    }

    // Get the selected DSPs from SQL Server
    public async Task<IReadOnlyList<DSP>> GetDspsByIdsAsync(
        IReadOnlyCollection<int> dspIds,
        CancellationToken cancellationToken = default)
    {
        return await _db.DSPs
            .AsNoTracking()
            .Where(d => dspIds.Contains(d.Id))
            .ToListAsync(cancellationToken);
    }

    // Save pending changes
    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }


}
