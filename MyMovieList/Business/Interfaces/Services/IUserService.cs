using MyMovieList.Data.Models;

namespace MyMovieList.Business.Interfaces.Services;

public interface IUserService
{
    Task Update(ApplicationUser user);
}
