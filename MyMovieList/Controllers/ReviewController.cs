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
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IMovieService _movieService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewController(
        IReviewService reviewService,
        IMovieService movieService,
        UserManager<ApplicationUser> userManager)
    {
        _reviewService = reviewService;
        _movieService = movieService;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetMovieReviews(Guid movieId, int page, int pageSize)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page number and page size should be greater than 0.");
        }

        if (!await _movieService.Exists(movieId))
        {
            return NotFound("The movie specified was not found.");
        }

        return Ok(await _reviewService.GetReviews(movieId, page, pageSize));
    }

    [AllowAnonymous]
    [HttpGet("[action]")]
    public async Task<IActionResult> GetUserReviews(string userId, int page, int pageSize)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Page number and page size should be greater than 0.");
        }

        if (await _userManager.FindByIdAsync(userId) is not null)
        {
            return NotFound("The user specified was not found.");
        }

        return Ok(await _reviewService.GetReviews(userId, page, pageSize));
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AddReview(Guid movieId, int vote, string? review)
    {
        if (!await _movieService.Exists(movieId))
        {
            return NotFound("The movie specified was not found.");
        }

        if (vote < 0 || vote > 10)
        {
            return BadRequest("The vote should be between 0 and 10.");
        }

        var loggedUserId = _userManager.GetUserId(User);
        var userWatchedMovie = _userManager.Users
            .Any(u => u.WatchList.Any(wli => wli.MovieId == movieId && wli.WatchStatus == WatchStatus.Watched));

        if (!userWatchedMovie)
        {
            return BadRequest("You need to have watched the movie to leave a review.");
        }

        var userAlreadyReviewedMovie = _userManager.Users
            .Any(u => u.Reviews.Any(mr => mr.MovieId == movieId));


        if (userAlreadyReviewedMovie)
        {
            return BadRequest("You have already reviewed this movie. Please edit your review.");
        }

        await _reviewService.AddReview(movieId, loggedUserId!, vote, review);

        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> EditReview(Guid movieId, int vote, string? review)
    {
        if (!await _movieService.Exists(movieId))
        {
            return NotFound("The movie specified was not found.");
        }

        if (vote < 0 || vote > 10)
        {
            return BadRequest("The vote should be between 0 and 10.");
        }

        var loggedUserId = _userManager.GetUserId(User);
        var userWatchedMovie = _userManager.Users
            .Any(u => u.WatchList.Any(wli => wli.MovieId == movieId && wli.WatchStatus == WatchStatus.Watched));

        if (!userWatchedMovie)
        {
            return BadRequest("You need to have watched the movie to leave a review.");
        }

        var userAlreadyReviewedMovie = _userManager.Users
            .Any(u => u.Reviews.Any(mr => mr.MovieId == movieId));


        if (!userAlreadyReviewedMovie)
        {
            return NotFound("You don't have a review for this movie yet. Please create one.");
        }

        await _reviewService.UpdateReview(movieId, loggedUserId!, vote, review);

        return Ok();
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> RemoveReview(Guid movieId)
    {
        if (!await _movieService.Exists(movieId))
        {
            return NotFound("The movie specified was not found.");
        }

        var loggedUserId = _userManager.GetUserId(User);
        var userWatchedMovie = _userManager.Users
            .Any(u => u.WatchList.Any(wli => wli.MovieId == movieId && wli.WatchStatus == WatchStatus.Watched));

        var userAlreadyReviewedMovie = _userManager.Users
            .Any(u => u.Reviews.Any(mr => mr.MovieId == movieId));


        if (!userAlreadyReviewedMovie)
        {
            return NotFound("You don't have a review for this movie.");
        }

        await _reviewService.DeleteReview(movieId, loggedUserId!);

        return Accepted();
    }
}