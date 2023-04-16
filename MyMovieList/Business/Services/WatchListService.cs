using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data;
using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Business.Services;

public class WatchListService : IWatchListService
{
    private readonly ApiDbContext _context;

    public WatchListService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task AddMovieToWatchList(string userId, Guid movieId, WatchStatus watchStatus)
    {
        if (!_context.WatchListItems.Any(wli => wli.MovieId == movieId && wli.UserId == userId))
        {
            _context.WatchListItems.Add(new WatchListItem
            {
                MovieId = movieId,
                UserId = userId,
                WatchStatus = watchStatus
            });

            await _context.SaveChangesAsync();
        }
    }

    public async Task EditWatchListItemStatus(string userId, Guid movieId, WatchStatus watchStatus)
    {
        var watchListItem = await _context.WatchListItems.FindAsync(userId, movieId);

        if (watchListItem is not null)
        {
            watchListItem.WatchStatus = watchStatus;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<WatchListItem>> GetUserWatchList(string userId, int page, int pageSize)
    {
        return await _context.WatchListItems.AsNoTracking()
            .Where(wli => wli.UserId == userId)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task RemoveMovieFromWatchList(string userId, Guid movieId)
    {
        var watchListItem = await _context.WatchListItems.FindAsync(userId, movieId);

        if (watchListItem is not null)
        {
            _context.WatchListItems.Remove(watchListItem);
            await _context.SaveChangesAsync();
        }
    }
}
