
using System.ComponentModel.DataAnnotations;

namespace TrackManagement.Application.DTOs.Tracks;

public class CreateTrackRequest
{
    [Required(ErrorMessage = "Track Title is required.")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1, int.MaxValue,
        ErrorMessage = "A valid artist ID is required.")]
    public int ArtistId { get; set; }

    [Required(ErrorMessage = "ISRC is required.")]
    [RegularExpression(
        @"^[A-Z]{2}[A-Z0-9]{3}[0-9]{7}$",
        ErrorMessage = "ISRC must contain 12 valid characters without hyphens.")]
    public string Isrc { get; set; } = string.Empty;

    [Required]
    public DateOnly? ReleaseDate { get; set; }

    [Required(ErrorMessage = "Genre is required.")]
    [MaxLength(100)]
    public string Genre { get; set; } = string.Empty;
    
}
