using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class WatchListController : ControllerBase
{
    private readonly IWatchListService _watchListService;
    private readonly IMovieService _movieService;
    private readonly UserManager<ApplicationUser> _userManager;

    public WatchListController(
        IWatchListService watchListService,
        IMovieService movieService,
        UserManager<ApplicationUser> userManager)
    {
        _watchListService = watchListService;
        _movieService = movieService;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetWatchList(string userId, int page, int pageSize)
    {
        if (page <= 0 && pageSize <= 0)
        {
            return BadRequest("Page number and page size should be greater than 0.");
        }

        if (await _userManager.FindByIdAsync(userId) is null)
        {
            return NotFound("The user specified was not found.");
        }

        return Ok(await _watchListService.GetUserWatchList(userId, page, pageSize));
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

        await _watchListService.AddMovieToWatchList(user!.Id, movieId, watchStatus);

        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> EditWatchListItemStatus(Guid movieId, WatchStatus watchStatus)
    {
        var user = await _userManager.GetUserAsync(User);
        var movie = await _movieService.Get(movieId);

        if (movie is null)
        {
            return NotFound("The movie specified was not found.");
        }

        await _watchListService.EditWatchListItemStatus(user!.Id, movieId, watchStatus);

        return Accepted();
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> RemoveFromWatchList(Guid movieId)
    {
        var user = await _userManager.GetUserAsync(User);
        var movie = await _movieService.Get(movieId);

        if (movie is null)
        {
            return NotFound("The movie specified was not found.");
        }

        await _watchListService.RemoveMovieFromWatchList(user!.Id, movieId);

        return Accepted();
    }
}