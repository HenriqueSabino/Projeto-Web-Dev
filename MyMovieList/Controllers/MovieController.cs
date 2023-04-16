using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMovieList.Business.Interfaces.Services;

namespace MyMovieList.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [AllowAnonymous]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetMovies(string? search, int page, int pageSize)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page number and page size should be greater than 0.");
        }

        return Ok(await _movieService.GetPaged(search, page, pageSize));
    }
}
