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
    /// Új felhasználói fiók regisztrálása
    /// </summary>
    /// <param name="body">Regisztrációhoz szükséges adatok</param>
    /// <response code="200">Sikeres regisztráció esetén</response>
    /// <response code="400">Ha a kérés tartalma hibás</response>
    /// <response code="409">Ha a megadott felhasználónév már foglalt</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] AuthRequestBody body)
    {
        if (body.Username.Length < 3 || body.Username.Length > 20)
        {
            return BadRequest(new ErrorResponse("A felhasználónév minimum 3, maximum 20 karakterből állhat."));
        }

        if (body.Password.Length < 6 || body.Password.Length > 32)
        {
            return BadRequest(new ErrorResponse("A jelszó minimum 6, maximum 32 karakterből állhat."));
        }

        bool success = await _authService.RegisterUser(body.Username, body.Password);
        if (!success)
        {
            return Conflict(new ErrorResponse("A felhasználó már létezik."));
        }

        return Ok(new SuccessResponse("Sikeres regisztráció!"));
    }

    /// <summary>
    /// Bejelentkezés egy meglévő felhasználói fiókba
    /// </summary>
    /// <param name="body">Bejelentkezéshez szükséges adatok</param>
    /// <response code="200">Sikeres bejelentkezés esetén</response>
    /// <response code="400">Ha a kérés tartalma hibás</response>
    /// <response code="401">Ha a felhasználónév vagy jelszó helytelen</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AuthRequestBody body)
    {
        if (string.IsNullOrEmpty(body.Username) || string.IsNullOrEmpty(body.Password))
        {
            return BadRequest(new ErrorResponse("Nincs minden mező kitöltve."));
        }

        var user = await _authService.AuthenticateUser(body.Username, body.Password);
        if (user == null)
        {
            return Unauthorized(new ErrorResponse("Hibás felhasználónév vagy jelszó."));
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


        return Ok(new SuccessResponse("Sikeres bejelentkezés!"));
    }

    /// <summary>
    /// Felhasználó access tokenjének frissítése
    /// </summary>
    /// <response code="200">Sikeres access token generálás esetén</response>
    /// <response code="400">Ha a kérés tartalma hibás</response>
    /// <response code="401">Ha a refresh token érvénytelen vagy lejárt</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshAccessToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new ErrorResponse("Nincs érvényes refresh token."));
        }

        var user = await _authService.CheckRefreshToken(refreshToken);
        if (user == null)
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Unauthorized(new ErrorResponse("A refresh token érvénytelen vagy lejárt."));
        }

        string newAccessToken = _authService.GenerateAccessToken(user);
        Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.Add(_tokenConfig.AccessTokenExpiration)
        });


        return Ok(new SuccessResponse("Új access token generálása megtörtént."));
    }

    /// <summary>
    /// Felhasználó kijelentkeztetése
    /// </summary>
    /// <response code="200">Sikeres kijelentkezés esetén</response>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (refreshToken != null)
        {
            await _authService.DeleteRefreshToken(refreshToken);
            Response.Cookies.Delete("refresh_token");
        }

        Response.Cookies.Delete("access_token");

        return Ok(new SuccessResponse("Sikeres kijelentkezés."));
    }
}
