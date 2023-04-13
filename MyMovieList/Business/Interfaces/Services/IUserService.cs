using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Business.Interfaces.Services;

public interface IUserService
{
    Task Update(ApplicationUser user);
}
