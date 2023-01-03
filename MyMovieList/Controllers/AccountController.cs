using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMovieList.Business.Globals;
using MyMovieList.Data.Models;
using MyMovieList.Models;

namespace MyMovieList.Controllers;

[ApiController]
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

    [HttpPost("[action]/{userId}")]
    public async Task<IActionResult> Delete(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            string errorMessage = string.Join("\n", result.Errors.Select(x => x.Description));
            return BadRequest(errorMessage);
        }

        return Ok();
    }
}