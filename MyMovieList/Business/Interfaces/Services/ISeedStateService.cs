using MyMovieList.Data.Models;

namespace MyMovieList.Business.Interfaces.Services;

public interface ISeedStateService
{
    Task<SeedState?> Get(int seedStateId);

    Task<SeedState?> GetBySource(string source);

    Task Add(SeedState seedState);

    Task Update(SeedState seedState);

    Task Delete(SeedState seedState);

    Task Delete(int seedStateId);
}
