
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


    public async Task<TrackResponse?> UpdateStatusAsync(
        int id,
        UpdateTrackStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        // Convert the status string into an enum
        var status = request.Status.Trim().ToLowerInvariant() switch
        {
            "draft" => TrackStatus.Draft,
            "submitted" => TrackStatus.Submitted,
            "distributed" => TrackStatus.Distributed,
            _ => throw new ArgumentException(
                "Status must be draft, submitted, or distributed.")
        };

        // Update the track in the database
        var updatedTrack = await _trackRepository.UpdateStatusAsync(
            id, status, cancellationToken);

        if (updatedTrack is null)
        {
            return null;
        }

        // Convert the updated entity into a response DTO
        return MapToResponse(updatedTrack);
    }



    public async Task<TrackDetailResponse?> DistributeAsync(
        int id,
        DistributeTrackRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate the DSP IDs
        var dspIds = request.DspIds;

        if (dspIds is null || dspIds.Count == 0)
        {
            throw new ArgumentException(
                "Select at least one DSP.");
        }

        if (dspIds.Any(dspId => dspId <= 0) ||
            dspIds.Distinct().Count() != dspIds.Count)
        {
            throw new ArgumentException(
                "DSP IDs must be positive and unique.");
        }

        // 2. Check that the track exists
        var track = await _trackRepository.GetForDistributionAsync(
            id, cancellationToken);

        if (track is null)
        {
            return null;
        }

        // 3. Verify that all selected DSPs exist
        var dsps = await _trackRepository.GetDspsByIdsAsync(
            dspIds, cancellationToken);

        if (dsps.Count != dspIds.Count)
        {
            throw new ArgumentException(
                "One or more selected DSPs do not exist.");
        }

        // 4. Prevent duplicate distribution records
        var existingDspIds = track.TrackDistributions
            .Select(d => d.DspId)
            .ToHashSet();

        if (dspIds.Any(dspId => existingDspIds.Contains(dspId)))
        {
            throw new InvalidOperationException(
                "Track has already been submitted to one or more selected DSPs.");
        }

        // 5. Create distribution records
        var submittedAt = DateTime.UtcNow;

        foreach (var dspId in dspIds)
        {
            track.TrackDistributions.Add(new TrackDistribution
            {
                TrackId = track.Id,
                DspId = dspId,
                SubmittedAt = submittedAt,
                Status = DistributionStatus.Pending
            });
        }

        // 6. Update the overall track status
        if (track.Status == TrackStatus.Draft)
        {
            track.Status = TrackStatus.Submitted;
        }

        // 7. Save changes to SQL Server
        await _trackRepository.SaveChangesAsync(
            cancellationToken);

        // 8. Return updated track details
        return await GetByIdAsync(id, cancellationToken);
    }


}
