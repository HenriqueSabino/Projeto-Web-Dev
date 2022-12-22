using System.ComponentModel.DataAnnotations;

namespace MyMovieList.Models;

#nullable disable
public class RefreshTokenRequest
{
    [Required]
    public string Token { get; set; }
}
