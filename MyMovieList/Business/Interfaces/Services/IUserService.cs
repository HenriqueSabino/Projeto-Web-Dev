using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Business.Interfaces.Services;

public interface IUserService
{
    Task Update(ApplicationUser user);

    Task AddMovieToWatchList(ApplicationUser user, Guid movidId, WatchStatus watchStatus);
}
