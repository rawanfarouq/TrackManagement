
using TrackManagement.Domain.Entities;

namespace TrackManagement.Application.Interfaces;


//interface says what operations must exist 

public interface IArtistRepository
{
    Task AddAsync(
        Artist artist,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Artist>> GetAllAsync(
        CancellationToken cancellationToken = default);

}
