using System.ComponentModel.DataAnnotations.Schema;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Data.Models;

#nullable disable
public class Movie
{
    public Guid Id { get; set; }

    public int ExternalId { get; set; }

    public string Source { get; set; }

    public string Summary { get; set; }

    public string Title { get; set; }

    public string ImagePath { get; set; }

    public float VoteAverage { get; set; }

    public int VoteCount { get; set; }

    public List<MovieReview> Reviews { get; set; }

    public List<WatchListItem> WatchListItems { get; set; }

    [NotMapped]
    public double LocalVoteAverage => Reviews.Average(mr => mr.Vote);

    [NotMapped]
    public int LocalVoteCount => Reviews.Count;

    [NotMapped]
    public int WatchedCount => WatchListItems.Count(wli => wli.WatchStatus == WatchStatus.Watched);
}