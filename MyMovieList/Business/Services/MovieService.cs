using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data;
using MyMovieList.Data.Models;

namespace MyMovieList.Business.Services;

public class MovieService : IMovieService
{
    private readonly ApiDbContext _context;

    public MovieService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<Movie?> Get(Guid movieId)
    {
        return await _context.Movies.FindAsync(movieId);
    }

    public async Task<IEnumerable<Movie>> GetPaged(int page, int pageSize)
    {
        return await _context.Movies.AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task Add(Movie movie)
    {
        await _context.Movies.AddAsync(movie);
        await _context.SaveChangesAsync();
    }

    public async Task AddRange(IEnumerable<Movie> movies)
    {
        await _context.Movies.AddRangeAsync(movies);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Movie movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Movie movie)
    {
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid movieId)
    {
        var movie = await Get(movieId);

        if (movie is not null)
        {
            await Delete(movie);
        }
    }
}
