using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMovieList.Business.Globals;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;

    public MovieController(IMovieService movieService, IUserService userService, UserManager<ApplicationUser> userManager)
    {
        _movieService = movieService;
        _userService = userService;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetMovies(int page, int pageSize)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest();
        }

        return Ok(await _movieService.GetPaged(page, pageSize));
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AddToWatchList(Guid movieId, WatchStatus watchStatus = WatchStatus.WantToWatch)
    {
        var user = await _userManager.GetUserAsync(User);
        var movie = await _movieService.Get(movieId);

        if (movie is null)
        {
            return NotFound("The movie specified was not found.");
        }

        await _userService.AddMovieToWatchList(user!, movieId, watchStatus);

        return Ok();
    }
}
