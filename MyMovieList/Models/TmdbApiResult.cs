using System.Text.Json.Serialization;

namespace MyMovieList.Models;

#nullable disable

public class TmdbApiResult<T>
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("results")]
    public List<T> Results { get; set; }
}