using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Data.Models;

#nullable disable

[PrimaryKey(nameof(this.UserId), nameof(this.MovieId))]
public class WatchListItem
{
    [ForeignKey(nameof(this.UserId))]
    public ApplicationUser User { get; set; }

    public string UserId { get; set; }

    [ForeignKey(nameof(this.MovieId))]
    public Movie Movie { get; set; }

    [Key, Column(Order = 1)]
    public Guid MovieId { get; set; }

    public WatchStatus WatchStatus { get; set; }
}