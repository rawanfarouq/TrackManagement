
using TrackManagement.Domain.Enums;

namespace TrackManagement.Domain.Entities;

public class TrackDistribution
{
    public int Id { get; set; }

    public int TrackId { get; set; }

    public int DspId { get; set; }

    public DateTime SubmittedAt { get; set; }

    public DistributionStatus Status { get; set; }
        = DistributionStatus.Pending;

    // Navigation properties
    public Track Track { get; set; } = null!;

    public DSP Dsp { get; set; } = null!;
}
