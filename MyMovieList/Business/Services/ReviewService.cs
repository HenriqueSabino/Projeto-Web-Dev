using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data;
using MyMovieList.Data.Models;

namespace MyMovieList.Business.Services;

public class ReviewService : IReviewService
{
    private readonly ApiDbContext _context;

    public ReviewService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MovieReview>> GetReviews(Guid movieId, int page, int pageSize)
    {
        return await _context.MovieReviews.AsNoTracking()
            .Where(mr => mr.MovieId == movieId)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovieReview>> GetReviews(string userId, int page, int pageSize)
    {
        return await _context.MovieReviews.AsNoTracking()
            .Where(mr => mr.UserId == userId)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddReview(Guid movieId, string userId, int vote, string? review)
    {
        _context.MovieReviews.Add(new MovieReview
        {
            MovieId = movieId,
            UserId = userId,
            Vote = vote,
            Review = review,
        });

        await _context.SaveChangesAsync();
    }

    public async Task UpdateReview(Guid movieId, string userId, int vote, string? review)
    {
        var reviewObj = await _context.MovieReviews
            .FirstAsync(mr => mr.MovieId == movieId && mr.UserId == userId);

        reviewObj.Vote = vote;
        reviewObj.Review = review;

        _context.MovieReviews.Update(reviewObj);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteReview(Guid movieId, string userId)
    {
        var reviewObj = await _context.MovieReviews
            .FirstAsync(mr => mr.MovieId == movieId && mr.UserId == userId);

        _context.MovieReviews.Remove(reviewObj);

        await _context.SaveChangesAsync();
    }
}