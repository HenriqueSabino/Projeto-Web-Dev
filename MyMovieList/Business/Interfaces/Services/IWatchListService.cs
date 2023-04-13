using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Business.Interfaces.Services;

public interface IWatchListService
{
    Task<IEnumerable<WatchListItem>> GetUserWatchList(string userId, int page, int pageSize);

    Task AddMovieToWatchList(string userId, Guid movidId, WatchStatus watchStatus);

    Task RemoveMovieFromWatchList(string userId, Guid movidId);
}
