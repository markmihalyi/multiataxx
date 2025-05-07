using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Mime;


namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
public class AuthController(IOptions<TokenConfig> tokenConfig, AuthService authService) : ControllerBase
{
    private readonly TokenConfig _tokenConfig = tokenConfig.Value;
    private readonly AuthService _authService = authService;

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="body">Data required for registration</param>
    /// <response code="200">If the registration is successful</response>
    /// <response code="400">If the request body is invalid</response>
    /// <response code="409">If the username is already taken</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] AuthRequestBody body)
    {
        if (body.Username.Length < 3 || body.Username.Length > 20)
        {
            return BadRequest(new ErrorResponse("Username should be between 3 and 20 characters."));
        }

        if (body.Password.Length < 6 || body.Password.Length > 32)
        {
            return BadRequest(new ErrorResponse("Password should be between 6 and 32 characters."));
        }

        bool success = await _authService.RegisterUser(body.Username, body.Password);
        if (!success)
        {
            return Conflict(new ErrorResponse("The user already exists."));
        }

        return Ok(new SuccessResponse("A new account has been created."));
    }

    /// <summary>
    /// Sign in to an existing user account
    /// </summary>
    /// <param name="body">Data required to log in</param>
    /// <response code="200">If the login is successful</response>
    /// <response code="400">If the request body is invalid</response>
    /// <response code="401">If the username or password is incorrect</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AuthRequestBody body)
    {
        if (string.IsNullOrEmpty(body.Username) || string.IsNullOrEmpty(body.Password))
        {
            return BadRequest(new ErrorResponse("Not all fields are filled in."));
        }

        var user = await _authService.AuthenticateUser(body.Username, body.Password);
        if (user == null)
        {
            return Unauthorized(new ErrorResponse("Incorrect username or password."));
        }

        string accessToken = _authService.GenerateAccessToken(user);
        Response.Cookies.Append("access_token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(_tokenConfig.AccessTokenExpiration)
        });

        string refreshToken = user.RefreshToken ??= string.Empty;
        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(_tokenConfig.RefreshTokenExpiration)
        });


        return Ok(new SuccessResponse("You have successfully logged in."));
    }

    /// <summary>
    /// Refresh user access token
    /// </summary>
    /// <response code="200">If access token generation is successful</response>
    /// <response code="400">If the request body is invalid</response>
    /// <response code="401">If the refresh token is invalid or expired</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshAccessToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new ErrorResponse("Refresh token is not valid."));
        }

        var user = await _authService.CheckRefreshToken(refreshToken);
        if (user == null)
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Unauthorized(new ErrorResponse("Refresh token is invalid or expired."));
        }

        string newAccessToken = _authService.GenerateAccessToken(user);
        Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(_tokenConfig.AccessTokenExpiration)
        });


        return Ok(new SuccessResponse("A new access token has been generated."));
    }

    /// <summary>
    /// Get details of user
    /// </summary>
    /// <response code="200">If the request was successful</response>
    /// <response code="401">If the user is not logged in</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserDetails()
    {
        var userIdentifier = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdentifier == null)
        {
            return Unauthorized(new ErrorResponse("You are not logged in."));
        }

        int userId = Convert.ToInt32(userIdentifier.Value);
        var user = await _authService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized(new ErrorResponse("You are not logged in."));
        }

        return Ok(new UserDetailsResponse(user.Id, user.Username));
    }

    /// <summary>
    /// Logout from an account
    /// </summary>
    /// <response code="200">If the logout was successful</response>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (refreshToken != null)
        {
            await _authService.DeleteRefreshToken(refreshToken);
            Response.Cookies.Delete("refresh_token");
        }

        Response.Cookies.Delete("access_token");

        return Ok(new SuccessResponse("You have successfully logged out."));
    }
}
