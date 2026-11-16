
using TrackManagement.Application.DTOs.Artists;

namespace TrackManagement.Application.Interfaces;

public interface IArtistService
{
    //CreateAsync() receives a CreateArtistRequest and returns an ArtistResponse.
    Task<ArtistResponse> CreateAsync(
        CreateArtistRequest request,
        CancellationToken cancellationToken = default);

    //GetAllAsync() retrieves all artists and returns a collection of ArtistResponse objects.
    Task<IReadOnlyList<ArtistResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
