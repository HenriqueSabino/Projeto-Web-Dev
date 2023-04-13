using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data;
using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Business.Services;

public class UserService : IUserService
{
    private readonly ApiDbContext _context;

    public UserService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task Update(ApplicationUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task AddMovieToWatchList(ApplicationUser user, Guid movieId, WatchStatus watchStatus)
    {
        ArgumentNullException.ThrowIfNull(user);

        user.WatchList ??= new List<WatchListItem>();

        if (!user.WatchList.Any(x => x.MovieId == movieId))
        {
            user.WatchList.Add(new WatchListItem
            {
                MovieId = movieId,
                UserId = user.Id,
                WatchStatus = watchStatus
            });
        }

        _context.Users.Update(user);

        await _context.SaveChangesAsync();
    }

    public async Task RemoveMovieFromWatchList(ApplicationUser user, Guid movieId)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (user.WatchList is null)
            return;

        user.WatchList.RemoveAll(x => x.MovieId == movieId);
        _context.Users.Update(user);

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<WatchListItem>> GetUserWatchList(string userId, int page, int pageSize)
    {
        return await _context.WatchListItems.AsNoTracking()
            .Where(wli => wli.UserId == userId)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToListAsync();
    }
}
