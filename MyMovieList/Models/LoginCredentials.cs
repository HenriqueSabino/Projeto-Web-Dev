using System.ComponentModel.DataAnnotations;

namespace MyMovieList.Models;

#nullable disable
public class LoginCredentials
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
