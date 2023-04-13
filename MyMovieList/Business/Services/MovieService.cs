using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data;
using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;
using MyMovieList.Models.DTO;

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

    public async Task<IEnumerable<MovieDTO>> GetPaged(string? search, int page, int pageSize)
    {
        var query = _context.Movies.AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(m => m.Title.Contains(search));
        }

        return await query
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .Select(m => new MovieDTO
            {
                Id = m.Id,
                ImagePath = m.ImagePath,
                Source = m.Source,
                Summary = m.Summary,
                Title = m.Title,
                VoteAverage = m.Reviews.Count > 0 ? m.Reviews.Average(x => x.Vote) : m.VoteAverage,
                VoteCount = m.Reviews.Count > 0 ? m.Reviews.Count : m.VoteCount,
                WatchedCount = m.WatchListItems.Count(wli => wli.WatchStatus == WatchStatus.Watched)
            })
            .ToListAsync();
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

    public async Task<bool> Exists(Guid movieId)
    {
        return await _context.Movies.FindAsync(movieId) is not null;
    }
}
