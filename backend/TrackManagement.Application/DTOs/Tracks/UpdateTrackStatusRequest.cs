
using System.ComponentModel.DataAnnotations;

namespace TrackManagement.Application.DTOs.Tracks;

public class UpdateTrackStatusRequest
{
    [Required(ErrorMessage = "Track status is required.")]
    [RegularExpression(
        "^(draft|submitted|distributed)$",
        ErrorMessage = "Status must be draft, submitted, or distributed.")]
    public string Status { get; set; } = string.Empty;
}
