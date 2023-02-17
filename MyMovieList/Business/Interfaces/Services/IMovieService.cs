using MyMovieList.Data.Models;

namespace MyMovieList.Business.Interfaces.Services;

public interface IMovieService
{
    Task<Movie?> Get(Guid movieId);

    Task<IEnumerable<Movie>> GetPaged(int page, int pageSize);

    Task Add(Movie movie);

    Task AddRange(IEnumerable<Movie> movies);

    Task Update(Movie movie);

    Task Delete(Movie movie);

    Task Delete(Guid movieId);
}
