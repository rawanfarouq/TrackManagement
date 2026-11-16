
using TrackManagement.Application.DTOs.Tracks;

namespace TrackManagement.Application.Interfaces;

public interface ITrackService
{
    Task<TrackResponse> CreateAsync(
        CreateTrackRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TrackResponse>> GetAllAsync(
        int? artistId,
        string? genre,
        string? status,
        CancellationToken cancellationToken = default);

    Task<TrackDetailResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
