
namespace TrackManagement.Domain.Entities;

public class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    // One artist can have many tracks (one-to-many relationship lel database ERP relationship)
    public ICollection<Track> Tracks { get; set; } = new List<Track>();
}
