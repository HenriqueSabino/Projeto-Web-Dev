using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyMovieList.Business.Configuration;
using MyMovieList.Business.Interfaces.Services;
using MyMovieList.Data.Models;
using MyMovieList.Models;
using MyMovieList.Models.DTO;

namespace MyMovieList.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtBearerTokenSettings _jwtBearerTokenSettings;

    public AuthController(IAuthService authService, IUserService userService, UserManager<ApplicationUser> userManager, IOptions<JwtBearerTokenSettings> jwtBearerTokenSettings)
    {
        _authService = authService;
        _userService = userService;
        _userManager = userManager;
        _jwtBearerTokenSettings = jwtBearerTokenSettings.Value;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> Login(LoginCredentials loginCredentials)
    {
        ApplicationUser? identityUser;
        if (!ModelState.IsValid || loginCredentials is null || (identityUser = await ValidateUser(loginCredentials)) is null)
        {
            return BadRequest(new { Success = false, Message = "Login failed! Check your username/email and password and try again." });
        }

        var tokens = await GenerateTokens(identityUser);

        if (string.IsNullOrEmpty(tokens.Item1) || string.IsNullOrEmpty(tokens.Item2))
        {
            return BadRequest(new { Success = false, Message = "Login failed, please try again later!" });
        }

        return Ok(new
        {
            Token = tokens.Item1,
            RefreshToken = tokens.Item2,
            Success = true,
            User = new UserAuthDTO
            {
                Id = identityUser.Id,
                Name = identityUser.UserName,
                Email = identityUser.Email
            },
            Message = "Login successful"
        });
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Success = false, Message = "Failed" });
        }

        var identityUser = _userManager.Users.Include(x => x.RefreshTokens).FirstOrDefault(x => x.RefreshTokens.Any(y => y.Token == refreshToken.Token));

        if (identityUser is null)
        {
            return NotFound(new { Success = false, Message = "Failed" });
        }

        // Get existing refresh token if it is valid and revoke it
        var existingRefreshToken = identityUser.RefreshTokens.First(x => x.Token == refreshToken.Token);

        if (!string.IsNullOrEmpty(existingRefreshToken.RevokedByIp) ||
            existingRefreshToken.RevokedOn != DateTime.MinValue ||
            existingRefreshToken.ExpiryOn <= DateTime.UtcNow)
        {
            return BadRequest(new { Success = false, Message = "Failed" });
        }

        // If user found, then revoke
        var revokedToken = await _authService.RevokeRefreshToken(identityUser, existingRefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString());

        if (revokedToken)
        {
            var newToken = await GenerateTokens(identityUser);

            if (string.IsNullOrEmpty(newToken.Item1) || string.IsNullOrEmpty(newToken.Item2))
            {
                return BadRequest(new { Success = false, Message = "Failed" });
            }

            return Ok(new { Token = newToken.Item1, RefreshToken = newToken.Item2, Success = true, Message = "Success" });
        }

        return BadRequest(new { Success = false, Message = "Failed" });
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RevokeToken(RefreshTokenRequest refreshToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Success = false, Message = "Failed" });
        }

        var identityUser = _userManager.Users.Include(x => x.RefreshTokens).FirstOrDefault(x => x.RefreshTokens.Any(y => y.Token == refreshToken.Token));

        if (identityUser is null)
        {
            return NotFound(new { Success = false, Message = "Failed" });
        }

        // Get existing refresh token if it is valid and revoke it
        var existingRefreshToken = identityUser.RefreshTokens.First(x => x.Token == refreshToken.Token);

        if (!string.IsNullOrEmpty(existingRefreshToken.RevokedByIp) ||
            existingRefreshToken.RevokedOn != DateTime.MinValue ||
            existingRefreshToken.ExpiryOn <= DateTime.UtcNow)
        {
            return BadRequest(new { Success = false, Message = "Failed" });
        }

        // If user found, then revoke
        var revokedToken = await _authService.RevokeRefreshToken(identityUser, existingRefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString());

        if (revokedToken)
        {
            return Ok(new { Success = true, Message = "Success" });
        }

        // Otherwise, return error
        return BadRequest(new { Success = false, Message = "Failed" });
    }

    [HttpPost]
    [Route("Logout")]
    public async Task<IActionResult> Logout()
    {
        var identityUser = await _userManager.Users.Include(x => x.RefreshTokens).FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

        if (identityUser is null)
        {
            return NotFound(new { Success = false, Message = "Failed" });
        }

        // Get existing refresh token if it is valid and revoke it
        var existingRefreshToken = identityUser.RefreshTokens.First(x => x.RevokedOn == DateTime.MinValue);

        if (!string.IsNullOrEmpty(existingRefreshToken.RevokedByIp) ||
            existingRefreshToken.RevokedOn != DateTime.MinValue ||
            existingRefreshToken.ExpiryOn <= DateTime.UtcNow)
        {
            return BadRequest(new { Success = false, Message = "Failed" });
        }

        var revokedToken = await _authService.RevokeRefreshToken(identityUser, existingRefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString());

        // audit
        await _authService.AddAuthHistory(new AuthHistory()
        {
            ApplicationUser = identityUser,
            ActionDate = DateTime.UtcNow,
            Action = "Logout",
        });

        return Ok(new { Token = string.Empty, Success = true, Message = "Logged Out" });
    }


    #region Private Methods
    private async Task<ApplicationUser?> ValidateUser(LoginCredentials loginCredentials)
    {
        var identityUser = await _userManager.FindByEmailAsync(loginCredentials.Email);

        if (identityUser != null)
        {
            var result = _userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash!, loginCredentials.Password);
            return result == PasswordVerificationResult.Failed ? null : identityUser;
        }

        return null;
    }

    private async Task<(string, string)> GenerateTokens(ApplicationUser identityUser)
    {
        // Generate access token
        string accessToken = GenerateAccessToken(identityUser);

        // Generate refresh token and set it to cookie
        var ipAddress = HttpContext.Connection.RemoteIpAddress!.ToString();
        var refreshToken = GenerateRefreshToken(ipAddress, identityUser.Id);

        // Set Refresh Token Cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        HttpContext.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

        // Save refresh token to database
        identityUser.RefreshTokens ??= new List<RefreshToken>();

        identityUser.LastLoginDate = DateTime.UtcNow;
        identityUser.RefreshTokens.Add(refreshToken);

        await _userService.Update(identityUser);

        return (accessToken, refreshToken.Token);
    }

    private string GenerateAccessToken(ApplicationUser identityUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var symmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtBearerTokenSettings.SecretKey));

        var currentRoles = _userManager.GetRolesAsync(identityUser).Result;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(JwtRegisteredClaimNames.NameId, identityUser.Id),
                    new Claim(JwtRegisteredClaimNames.Email, identityUser.Email!),
                    new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, currentRoles.First()),
                }),

            Expires = DateTime.UtcNow.AddSeconds(_jwtBearerTokenSettings.ExpiryTimeInSeconds),
            SigningCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature),
            Audience = _jwtBearerTokenSettings.Audience,
            Issuer = _jwtBearerTokenSettings.Issuer,
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    private RefreshToken GenerateRefreshToken(string ipAddress, string userId)
    {
        using var rngCryptoServiceProvider = RandomNumberGenerator.Create();
        var randomBytes = new byte[64];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            ExpiryOn = DateTime.UtcNow.AddDays(_jwtBearerTokenSettings.RefreshTokenExpiryInDays),
            CreatedOn = DateTime.UtcNow,
            CreatedByIp = ipAddress,
            UserId = userId
        };
    }

    #endregion
}
