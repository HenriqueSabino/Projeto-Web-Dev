using System.ComponentModel.DataAnnotations.Schema;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Models.DTO;

#nullable disable
public class MovieDTO
{
    public Guid Id { get; set; }

    public string Source { get; set; }

    public string Summary { get; set; }

    public string Title { get; set; }

    public string ImagePath { get; set; }

    public double VoteAverage { get; set; }

    public int VoteCount { get; set; }

    [NotMapped]
    public int WatchedCount { get; set; }
}