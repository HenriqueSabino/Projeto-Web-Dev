using MyMovieList.Data.Models;

namespace MyMovieList.Business.Interfaces.Services;

public interface IReviewService
{
    Task<IEnumerable<MovieReview>> GetReviews(Guid movieId, int page, int pageSize);

    Task<IEnumerable<MovieReview>> GetReviews(string userId, int page, int pageSize);

    Task AddReview(Guid movieId, string userId, int vote, string? review);

    Task UpdateReview(Guid movieId, string userId, int vote, string? review);

    Task DeleteReview(Guid movieId, string userId);
}
