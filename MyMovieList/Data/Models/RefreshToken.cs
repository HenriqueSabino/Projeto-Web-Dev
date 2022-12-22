using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMovieList.Data.Models;

#nullable disable
public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    public string Token { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }

    public string UserId { get; set; }

    public DateTime ExpiryOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedByIp { get; set; }

    public DateTime RevokedOn { get; set; }

    public string RevokedByIp { get; set; }
}
