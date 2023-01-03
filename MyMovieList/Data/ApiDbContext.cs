using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Extensions;
using MyMovieList.Data.Models;

namespace MyMovieList.Data;

#nullable disable
public class ApiDbContext : IdentityDbContext<ApplicationUser>
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
    }

    public DbSet<AuthHistory> AuthHistories { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(t => t.User)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AuthHistory>()
            .HasOne(ah => ah.ApplicationUser)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.SeedUserRoles();
    }
}