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

    public DbSet<Movie> Movies { get; set; }

    public DbSet<WatchListItem> WatchListItems { get; set; }

    public DbSet<MovieReview> MovieReviews { get; set; }

    public DbSet<SeedState> SeedStates { get; set; }

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

        builder.Entity<WatchListItem>()
            .HasOne(wli => wli.User)
            .WithMany(u => u.WatchList)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<WatchListItem>()
            .HasOne(u => u.Movie)
            .WithMany(m => m.WatchListItems)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MovieReview>()
            .HasOne(mr => mr.User)
            .WithMany(u => u.Reviews)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<MovieReview>()
            .HasOne(mr => mr.Movie)
            .WithMany(m => m.Reviews)
            .OnDelete(DeleteBehavior.Cascade);

        builder.SeedUserRoles();
    }
}