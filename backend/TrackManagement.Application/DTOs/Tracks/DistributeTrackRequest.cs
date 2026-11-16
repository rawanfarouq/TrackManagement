
using System.ComponentModel.DataAnnotations;

namespace TrackManagement.Application.DTOs.Tracks;

public class DistributeTrackRequest
{
    [Required(ErrorMessage = "DSP IDs are required.")]
    [MinLength(1, ErrorMessage = "Select at least one DSP.")]
    public List<int> DspIds { get; set; } = new();
}
