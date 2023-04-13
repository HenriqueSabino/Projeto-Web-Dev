using MyMovieList.Data.Models;
using MyMovieList.Models.DTO;

namespace MyMovieList.Business.Interfaces.Services;

public interface IMovieService
{
    Task<Movie?> Get(Guid movieId);

    Task<IEnumerable<MovieDTO>> GetPaged(string? search, int page, int pageSize);

    Task Add(Movie movie);

    Task AddRange(IEnumerable<Movie> movies);

    Task Update(Movie movie);

    Task Delete(Movie movie);

    Task Delete(Guid movieId);

    Task<bool> Exists(Guid movieId);
}
