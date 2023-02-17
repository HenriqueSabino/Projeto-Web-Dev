using MyMovieList.Business.Configuration;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data;
using MyMovieList.Data.Models;
using MyMovieList.Models;
using Newtonsoft.Json;

namespace MyMovieList.HostedServices;

public class MovieSeedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TmdbApiSettings _tmdbApiSettings;
    private readonly ILogger<MovieSeedService> _logger;

    public MovieSeedService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<MovieSeedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var tmdSection = configuration.GetSection("Tmdb");
        _tmdbApiSettings = tmdSection.Get<TmdbApiSettings>()!;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Check if we need to start over from the beginning or continue from the last page seeded
                int page = 1;
                int totalPages = -1;
                int year = 1970;
                SeedState? seedState;

                using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    var seedStateService = scope.ServiceProvider.GetService<ISeedStateService>()!;
                    seedState = await seedStateService.GetBySource(_tmdbApiSettings.BaseUrl);
                }

                if (seedState != null)
                {
                    page = seedState.LastPage + 1;
                    totalPages = seedState.TotalPages;
                    year = seedState.Year;
                }

                // Query the TMDB API for movies
                while (totalPages < 0 || page <= totalPages)
                {
                    using var httpClient = new HttpClient();

                    var results = await httpClient.GetFromJsonAsync<TmdbApiResult<TmdbMovie>>(
                        $"{_tmdbApiSettings.BaseUrl}/discover/movie?api_key={_tmdbApiSettings.ApiKey}&page={page}&language=pt-BR&sort_by=primary_release_date.asc&year=${year}&vote_average.gte=6.1",
                         cancellationToken);
                    totalPages = results!.TotalPages;

                    var movies = new List<Movie>();
                    movies.AddRange(results.Results.Select(x => new Movie
                    {
                        ExternalId = x.Id,
                        Title = x.Title,
                        ImagePath = x.PosterPath,
                        Source = _tmdbApiSettings.BaseUrl,
                        VoteAverage = x.VoteAverage,
                        VoteCount = x.VoteCount,
                    }));

                    page++;

                    using var scope = _serviceProvider.CreateAsyncScope();
                    var seedStateService = scope.ServiceProvider.GetService<ISeedStateService>();
                    var movieService = scope.ServiceProvider.GetService<IMovieService>();

                    // Update the seed state with the last page seeded
                    if (seedState is null)
                    {
                        seedState = new SeedState
                        {
                            Source = _tmdbApiSettings.BaseUrl,
                            Year = year,
                        };

                        await seedStateService!.Add(seedState);
                    }

                    seedState.LastPage = page - 1;
                    seedState.TotalPages = totalPages;

                    if (seedState.LastPage == seedState.TotalPages)
                    {
                        seedState.LastPage = 0;
                        seedState.TotalPages = -1;
                        seedState.Year++;
                    }

                    await movieService!.AddRange(movies);
                    await seedStateService!.Update(seedState);

                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // Log the error but keep running
                // You can also throw an exception to stop the service if you prefer
                _logger.LogError(ex, "An error occurred while seeding the database.");
                await Task.Delay(TimeSpan.FromMinutes(20), cancellationToken);
            }
        }
    }
}