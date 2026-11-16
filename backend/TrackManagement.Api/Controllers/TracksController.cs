
using Microsoft.AspNetCore.Mvc;
using TrackManagement.Application.DTOs.Tracks;
using TrackManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TrackManagement.Api.Controllers;

[ApiController]
[Route("api/tracks")]
public class TracksController : ControllerBase
{
    private readonly ITrackService _trackService;

    public TracksController(ITrackService trackService)
    {
        _trackService = trackService;
    }

    // POST /api/tracks
    [HttpPost]
    public async Task<ActionResult<TrackResponse>> Create(
        [FromBody] CreateTrackRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var track = await _trackService.CreateAsync(
                request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = track.Id },
                track);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
            when (ex.Message == "A track with this ISRC already exists.")
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // GET /api/tracks?artistId=&genre=&status=
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TrackResponse>>> GetAll(
        [FromQuery] int? artistId,
        [FromQuery] string? genre,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        try
        {
            var tracks = await _trackService.GetAllAsync(
                artistId, genre, status, cancellationToken);

            return Ok(tracks);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // GET /api/tracks/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TrackDetailResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var track = await _trackService.GetByIdAsync(
            id, cancellationToken);

        if (track is null)
        {
            return NotFound(new
            {
                message = "Track not found."
            });
        }

        return Ok(track);
    }


    // PATCH /api/tracks/{id}/status
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<TrackResponse>> UpdateStatus(
        int id,
        [FromBody] UpdateTrackStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updatedTrack = await _trackService.UpdateStatusAsync(
                id, request, cancellationToken);

            if (updatedTrack is null)
            {
                return NotFound(new
                {
                    message = "Track not found."
                });
            }

            return Ok(updatedTrack);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // POST /api/tracks/{id}/distribute
    [Authorize]
    [HttpPost("{id:int}/distribute")]
    public async Task<ActionResult<TrackDetailResponse>> Distribute(
        int id,
        [FromBody] DistributeTrackRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _trackService.DistributeAsync(
                id, request, cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = "Track not found."
                });
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
            when (ex.Message ==
                "Track has already been submitted to one or more selected DSPs.")
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }


}
