using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Globals;

namespace MyMovieList.Business.Extensions;

public static class ModelBuilderExtensions
{
    public static void SeedUserRoles(this ModelBuilder builder)
    {
        var roles = new List<IdentityRole>
        {
            new IdentityRole()
            {
                Id = "61311391-a41d-4101-a619-9e044242fe24",
                Name = UserRoles.SuperAdmin,
                NormalizedName = UserRoles.SuperAdmin.ToUpper(),
            },
            new IdentityRole()
            {
                Id = "e7254eb3-bb78-41a7-bb81-2383ce4ab780",
                Name = UserRoles.Admin,
                NormalizedName = UserRoles.Admin.ToUpper(),
            },
            new IdentityRole()
            {
                Id = "f6eef6ba-fb15-4095-bcad-ee882169d9f9",
                Name = UserRoles.User,
                NormalizedName = UserRoles.User.ToUpper(),
            }
        };

        builder.Entity<IdentityRole>().HasData(roles);
    }
}