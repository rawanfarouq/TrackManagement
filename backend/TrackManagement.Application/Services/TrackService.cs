
using TrackManagement.Application.DTOs.Tracks;
using TrackManagement.Application.Interfaces;
using TrackManagement.Domain.Entities;
using TrackManagement.Domain.Enums;

namespace TrackManagement.Application.Services;

public class TrackService : ITrackService
{
    private readonly ITrackRepository _trackRepository;

    public TrackService(ITrackRepository trackRepository)
    {
        _trackRepository = trackRepository;
    }

    // CREATE TRACK
    public async Task<TrackResponse> CreateAsync(
        CreateTrackRequest request,
        CancellationToken cancellationToken = default)
    {
        // Check that the artist exists
        if (!await _trackRepository.ArtistExistsAsync(
            request.ArtistId, cancellationToken))
        {
            throw new ArgumentException(
                "The specified artist does not exist.");
        }

        var isrc = request.Isrc.Trim();

        // Check for duplicate ISRC
        if (await _trackRepository.IsrcExistsAsync(
            isrc, cancellationToken))
        {
            throw new InvalidOperationException(
                "A track with this ISRC already exists.");
        }

        var track = new Track
        {
            Title = request.Title.Trim(),
            ArtistId = request.ArtistId,
            Isrc = isrc,
            ReleaseDate = request.ReleaseDate!.Value,
            Genre = request.Genre.Trim(),
            Status = TrackStatus.Draft
        };

        await _trackRepository.AddAsync(
            track, cancellationToken);

        // Reload the track to include its artist
        var savedTrack = await _trackRepository.GetByIdAsync(
            track.Id, cancellationToken);

        if (savedTrack is null)
        {
            throw new InvalidOperationException(
                "The saved track could not be retrieved.");
        }

        return MapToResponse(savedTrack);
    }

    // GET ALL TRACKS WITH FILTERS
    public async Task<IReadOnlyList<TrackResponse>> GetAllAsync(
        int? artistId,
        string? genre,
        string? status,
        CancellationToken cancellationToken = default)
    {
        TrackStatus? parsedStatus = status?.Trim().ToLowerInvariant() switch
        {
            null or "" => null,
            "draft" => TrackStatus.Draft,
            "submitted" => TrackStatus.Submitted,
            "distributed" => TrackStatus.Distributed,
            _ => throw new ArgumentException(
                "Status must be draft, submitted, or distributed.")
        };

        var tracks = await _trackRepository.GetAllAsync(
            artistId,
            genre,
            parsedStatus,
            cancellationToken);

        return tracks.Select(MapToResponse).ToList();
    }

    // GET TRACK DETAILS
    public async Task<TrackDetailResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var track = await _trackRepository.GetByIdAsync(
            id, cancellationToken);

        if (track is null)
            return null;

        return new TrackDetailResponse
        {
            Id = track.Id,
            Title = track.Title,
            ArtistId = track.ArtistId,
            ArtistName = track.Artist.Name,
            Isrc = track.Isrc,
            ReleaseDate = track.ReleaseDate,
            Genre = track.Genre,
            Status = track.Status.ToString().ToLowerInvariant(),

            Distributions = track.TrackDistributions
                .OrderBy(d => d.DspId)
                .Select(d => new TrackDistributionResponse
                {
                    Id = d.Id,
                    DspId = d.DspId,
                    DspName = d.Dsp.Name,
                    SubmittedAt = d.SubmittedAt,
                    Status = d.Status.ToString().ToLowerInvariant()
                })
                .ToList()
        };
    }

    // MAP TRACK ENTITY TO RESPONSE DTO
    private static TrackResponse MapToResponse(Track track)
    {
        return new TrackResponse
        {
            Id = track.Id,
            Title = track.Title,
            ArtistId = track.ArtistId,
            ArtistName = track.Artist.Name,
            Isrc = track.Isrc,
            ReleaseDate = track.ReleaseDate,
            Genre = track.Genre,
            Status = track.Status.ToString().ToLowerInvariant()
        };
    }
}
