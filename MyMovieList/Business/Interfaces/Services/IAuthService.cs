using MyMovieList.Data.Models;

namespace MyMovieList.Business.Interfaces.Services;

public interface IAuthService
{
    Task AddAuthHistory(AuthHistory authHistory);
    Task<bool> RevokeRefreshToken(ApplicationUser user, RefreshToken refreshToken, string? remoteIpAddress);
}
