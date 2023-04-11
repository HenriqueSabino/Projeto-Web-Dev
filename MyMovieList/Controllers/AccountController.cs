using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyMovieList.Business.Globals;
using MyMovieList.Data.Models;
using MyMovieList.Models;
using MyMovieList.Models.DTO;

namespace MyMovieList.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> Register(UserCreationDTO userCreationDTO)
    {
        var user = await _userManager.GetUserAsync(User);

        var userRole = userCreationDTO.UserRole;
        if (!await _roleManager.RoleExistsAsync(userRole))
        {
            return BadRequest("The specified user role does not exist.");
        }

        if (userRole == UserRoles.Admin || userRole == UserRoles.SuperAdmin)
        {
            if (user is null || !await _userManager.IsInRoleAsync(user!, UserRoles.SuperAdmin))
            {
                return Unauthorized("Only Super Admins can create new system admins.");
            }
        }

        var newUser = new ApplicationUser
        {
            Email = userCreationDTO.Email,
            UserName = userCreationDTO.UserName,
        };

        var creationResult = await _userManager.CreateAsync(newUser, userCreationDTO.Password);
        if (!creationResult.Succeeded)
        {
            string errorMessage = string.Join("\n", creationResult.Errors.Select(x => x.Description));
            return BadRequest(errorMessage);
        }

        var roleResult = await _userManager.AddToRoleAsync(newUser, userCreationDTO.UserRole);
        if (!roleResult.Succeeded)
        {
            string errorMessage = string.Join("\n", roleResult.Errors.Select(x => x.Description));
            return BadRequest(errorMessage);
        }

        return Ok();
    }

    [HttpDelete("[action]/{userId}")]
    public async Task<IActionResult> Delete(string userId)
    {
        var loggedUser = await _userManager.GetUserAsync(User);
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        if (!await _userManager.IsInRoleAsync(loggedUser!, UserRoles.SuperAdmin) ||
            !await _userManager.IsInRoleAsync(loggedUser!, UserRoles.Admin) ||
            (await _userManager.IsInRoleAsync(loggedUser!, UserRoles.Admin) && await _userManager.IsInRoleAsync(user, UserRoles.Admin)) ||
            loggedUser!.Id != userId)
        {
            return Unauthorized("Only the proprietor of the account or users with higher priviledges can delete this account.");
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            string errorMessage = string.Join("\n", result.Errors.Select(x => x.Description));
            return BadRequest(errorMessage);
        }

        return Ok();
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetWatchList()
    {
        var loggedUserId = _userManager.GetUserId(User);
        var loggedUser = await _userManager.Users
            .Include(x => x.WatchList)
            .ThenInclude(x => x.Movie)
            .SingleOrDefaultAsync(x => x.Id == loggedUserId);

        var watchListDTO = loggedUser!.WatchList?.Select(x => new WatchListDTO
        {
            Movie = x.Movie,
            WatchStatus = x.WatchStatus
        });

        return Ok(watchListDTO ?? new List<WatchListDTO>());
    }
}