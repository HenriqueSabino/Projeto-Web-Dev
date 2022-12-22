using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Globals;
using MyMovieList.Data;
using MyMovieList.Data.Models;

namespace MyMovieList.Business.Configuration;

public class SeedData
{
    private readonly ApiDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<SeedData> _logger;

    public SeedData(ApiDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<SeedData> logger)
    {
        _context = context;
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAllDataASync()
    {
        await _context.Database.MigrateAsync();
        await SeedInitialData();
    }

    private async Task SeedInitialData()
    {
        try
        {
            #region Seeding User Roles

            

            #endregion

            #region Seeding System User
            // Seed Development User
            var systemAdminAppUser = new ApplicationUser
            {
                Id = "26ab1151-15eb-49a7-81ca-d25934dfd51b",
                UserName = _configuration["Admin:UserName"],
                Email = _configuration["Admin:Email"],
                EmailConfirmed = true,
            };

            await AddUserAsync(systemAdminAppUser, new List<string>() { UserRoles.SuperAdmin });

            #endregion
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while seeding the initial data");
            throw;
        }
    }

    private async Task AddUserAsync(ApplicationUser applicationUser, List<string> roles)
    {
        // Dont want to reseed existing users
        ApplicationUser? userResult;
        if (string.IsNullOrWhiteSpace(applicationUser.Id))
        {
            userResult = await _userManager.FindByEmailAsync(applicationUser.Email!);
        }
        else
        {
            userResult = await _userManager.FindByIdAsync(applicationUser.Id);
        }

        if (userResult == null)
        {
            userResult = applicationUser;
            IdentityResult result = await _userManager.CreateAsync(userResult, _configuration["Admin:Password"]!);
            if (result.Succeeded == false)
            {
                string err = $"{nameof(AddUserAsync)}. Failed to create User: {userResult.Email}.";
                throw new Exception(err);
            }
        }

        // Add Role to User if not already done
        foreach (string role in roles)
        {
            var roleDb = await _roleManager.FindByNameAsync(role.ToUpper());

            if (roleDb is not null && !await _userManager.IsInRoleAsync(userResult, roleDb!.Name!))
            {
                var roleResult = await _userManager.AddToRoleAsync(userResult, roleDb.Name!);
                if (!roleResult.Succeeded)
                {
                    string err = $"{nameof(AddUserAsync)}. Failed to add Role {roleDb.Name} to User: {userResult.Email}.";
                    throw new Exception(err);
                }
            }
        }
    }
}