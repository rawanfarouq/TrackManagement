
using Microsoft.AspNetCore.Mvc;
using TrackManagement.Application.DTOs.Artists;
using TrackManagement.Application.Interfaces;

namespace TrackManagement.Api.Controllers;

[ApiController]
[Route("api/artists")]
public class ArtistsController : ControllerBase
{
    private readonly IArtistService _artistService;

    public ArtistsController(IArtistService artistService)
    {
        _artistService = artistService;
    }

    // POST /api/artists
    [HttpPost]
    public async Task<ActionResult<ArtistResponse>> Create(
        [FromBody] CreateArtistRequest request,
        CancellationToken cancellationToken)
    {
        var artist = await _artistService.CreateAsync(
            request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, artist);
    }

    // GET /api/artists
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ArtistResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var artists = await _artistService.GetAllAsync(
            cancellationToken);

        return Ok(artists);
    }
}
