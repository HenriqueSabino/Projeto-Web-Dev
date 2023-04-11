using MyMovieList.Data.Models;
using MyMovieList.Data.Models.Enums;

namespace MyMovieList.Models.DTO;

#nullable disable
public class WatchListDTO
{
    public Movie Movie { get; set; }

    public WatchStatus WatchStatus { get; set; }
}