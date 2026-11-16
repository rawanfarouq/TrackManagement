
namespace TrackManagement.Application.DTOs.Tracks;

public class TrackDistributionResponse
{
    public int Id { get; set; }

    public int DspId { get; set; }

    public string DspName { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }

    public string Status { get; set; } = string.Empty;
}
