
using System.ComponentModel.DataAnnotations;

namespace TrackManagement.Application.DTOs.Artists;

public class CreateArtistRequest
{
    [Required(ErrorMessage = "Artist name is required.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required.")]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
}
