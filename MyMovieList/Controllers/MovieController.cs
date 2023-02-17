using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMovieList.Business.Globals;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data.Models;

namespace MyMovieList.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MovieController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetMovies(int page, int pageSize)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest();
        }

        return Ok(await _movieService.GetPaged(page, pageSize));
    }
}
