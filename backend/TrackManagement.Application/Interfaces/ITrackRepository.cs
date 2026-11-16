
using TrackManagement.Domain.Entities;
using TrackManagement.Domain.Enums;

namespace TrackManagement.Application.Interfaces;

public interface ITrackRepository
{
    // Create a new track
    Task AddAsync(
        Track track,
        CancellationToken cancellationToken = default);

    // Get all tracks with optional filters
    Task<IReadOnlyList<Track>> GetAllAsync(
        int? artistId,
        string? genre,
        TrackStatus? status,
        CancellationToken cancellationToken = default);

    // Get one track, including its distributions
    Task<Track?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    // Check if an artist exists
    Task<bool> ArtistExistsAsync(
        int artistId,
        CancellationToken cancellationToken = default);

    // Check if an ISRC is already registered
    Task<bool> IsrcExistsAsync(
        string isrc,
        CancellationToken cancellationToken = default);

    Task<Track?> UpdateStatusAsync(
        int id,
        TrackStatus status,
        CancellationToken cancellationToken = default);

    
    Task<Track?> GetForDistributionAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DSP>> GetDspsByIdsAsync(
        IReadOnlyCollection<int> dspIds,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
    
        
}
