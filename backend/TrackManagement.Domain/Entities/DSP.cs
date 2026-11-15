
namespace TrackManagement.Domain.Entities;

public class DSP
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // One DSP can receive many track distributions
    public ICollection<TrackDistribution> TrackDistributions
        { get; set; } = new List<TrackDistribution>();
}
