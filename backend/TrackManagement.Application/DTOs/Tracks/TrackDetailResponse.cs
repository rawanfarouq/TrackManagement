
namespace TrackManagement.Application.DTOs.Tracks;

public class TrackDetailResponse : TrackResponse
{
    public List<TrackDistributionResponse> Distributions
        { get; set; } = new();
}
