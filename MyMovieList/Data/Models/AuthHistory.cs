using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMovieList.Data.Models;

#nullable disable
public class AuthHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; }

    [Required]
    public string ApplicationUserId { get; set; }

    public DateTime ActionDate { get; set; }

    public string Action { get; set; }
}
