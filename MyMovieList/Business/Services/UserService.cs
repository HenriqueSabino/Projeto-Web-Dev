using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data;
using MyMovieList.Data.Models;

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
}
