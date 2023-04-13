using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Business.Interfaces.Services;

public interface IUserService
{
    Task<IEnumerable<WatchListItem>> GetUserWatchList(string userId, int page, int pageSize);

    Task Update(ApplicationUser user);

    Task AddMovieToWatchList(ApplicationUser user, Guid movidId, WatchStatus watchStatus);

    Task RemoveMovieFromWatchList(ApplicationUser user, Guid movidId);
}
