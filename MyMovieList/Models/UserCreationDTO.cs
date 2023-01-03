using MyMovieList.Business.Globals;

namespace MyMovieList.Models;

#nullable disable
public class UserCreationDTO
{
    public string Email { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public string UserRole { get; set; }
}