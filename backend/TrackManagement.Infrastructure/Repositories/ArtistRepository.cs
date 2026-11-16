
using Microsoft.EntityFrameworkCore;
using TrackManagement.Application.Interfaces;
using TrackManagement.Domain.Entities;
using TrackManagement.Infrastructure.Data;

namespace TrackManagement.Infrastructure.Repositories;

public class ArtistRepository : IArtistRepository
{
    private readonly TrackManagementDbContext _db;

    public ArtistRepository(TrackManagementDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(
        Artist artist,
        CancellationToken cancellationToken = default)
    {
        await _db.Artists.AddAsync(artist, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Artist>> GetAllAsync(
        CancellationToken cancellationToken = default)  //It allows an operation to be cancelled when an HTTP request is aborted.
    {
        return await _db.Artists  //access the artist entities
            .AsNoTracking()       // tells EF core not to track the changes made to the entities.
            .OrderBy(a => a.Id)   // sort entities by id 
            .ToListAsync(cancellationToken);
    }
}
