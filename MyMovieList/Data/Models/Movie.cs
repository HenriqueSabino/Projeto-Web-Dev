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
}