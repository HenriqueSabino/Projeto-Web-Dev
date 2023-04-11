using Microsoft.AspNetCore.Identity;

namespace MyMovieList.Data.Models;

#nullable disable
public class ApplicationUser : IdentityUser
{

    public List<RefreshToken> RefreshTokens { get; set; }

    public DateTime LastLoginDate { get; set; }

    public DateTime LastLogoutDate { get; internal set; }

    public List<WatchListItem> WatchList { get; set; }
}