using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data;
using MyMovieList.Data.Models;

namespace MyMovieList.Business.Services;

public class AuthService : IAuthService
{
    private readonly ApiDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApiDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task AddAuthHistory(AuthHistory authHistory)
    {
        _context.AuthHistories.Add(authHistory);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> RevokeRefreshToken(ApplicationUser user, RefreshToken refreshToken, string? remoteIpAddress)
    {
        user.LastLogoutDate = DateTime.UtcNow;
        refreshToken.RevokedByIp = remoteIpAddress;
        refreshToken.RevokedOn = DateTime.UtcNow;

        try
        {
            _context.Users.Update(user);
            _context.RefreshTokens.Update(refreshToken);

            await _context.SaveChangesAsync();
        } catch (Exception e)
        {
            _logger.LogError(e, "Could not update user {UserId} and/or refresh token {RefreshId}", user.Id, refreshToken.Id);
            return false;
        }

        return true;
    }
}
