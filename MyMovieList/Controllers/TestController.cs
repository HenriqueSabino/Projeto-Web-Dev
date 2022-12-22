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
    public IActionResult LoggedIn()
    {
        return Ok(new { Message = $"Logged user is {_userManager.GetUserName(User)} " });
    }

    [Authorize(Roles = UserRoles.SuperAdmin)]
    [HttpGet("[action]")]
    public IActionResult SuperAdmin()
    {
        return Ok(new { Message = $"SuperAdmin user is {_userManager.GetUserName(User)} " });
    }
}
