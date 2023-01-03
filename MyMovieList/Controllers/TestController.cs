using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMovieList.Business.Globals;
using MyMovieList.Data.Models;

namespace MyMovieList.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public TestController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("[action]")]
    public IActionResult Anonymous()
    {
        return Ok(new { Message = "Anonymous user (no login required)" });
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> LoggedIn()
    {
        var user = await _userManager.GetUserAsync(User);
        return Ok(new { Message = $"Logged user is {user!.UserName}" });
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet("[action]")]
    public async Task<IActionResult> SuperAdmins()
    {
        var user = await _userManager.GetUserAsync(User);
        return Ok(new { Message = $"SuperAdmin user is {user!.UserName}" });
    }

    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin}")]
    [HttpGet("[action]")]
    public async Task<IActionResult> Admins()
    {
        var user = await _userManager.GetUserAsync(User);
        return Ok(new { Message = $"Admin user is {user!.UserName}" });
    }

    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.User}")]
    [HttpGet("[action]")]
    public async Task<IActionResult> Users()
    {
        var user = await _userManager.GetUserAsync(User);
        return Ok(new { Message = $"User is {user!.UserName}" });
    }
}
