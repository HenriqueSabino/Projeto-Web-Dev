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
}
