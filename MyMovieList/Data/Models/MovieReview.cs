using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMovieList.Data.Models;

#nullable disable
public class MovieReview
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(this.UserId))]
    public ApplicationUser User { get; set; }

    public string UserId { get; set; }

    [ForeignKey(nameof(this.MovieId))]
    public Movie Movie { get; set; }

    public Guid MovieId { get; set; }

    public int Vote { get; set; }

    public string Review { get; set; }
}
