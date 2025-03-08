using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;


namespace Backend.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserAuthDto dto)
    {
        if (dto.Username.Length < 3 || dto.Username.Length > 20)
            return BadRequest(new { success = false, message = "A felhasználónév minimum 3, maximum 20 karakterből állhat." });

        if (dto.Password.Length < 6 || dto.Password.Length > 32)
            return BadRequest(new { success = false, message = "A jelszó minimum 6, maximum 32 karakterből állhat." });

        bool success = await _authService.RegisterUser(dto.Username, dto.Password);
        if (!success)
            return Conflict(new { success = false, message = "A felhasználó már létezik." });

        return Ok(new { success = true, message = "Sikeres regisztráció!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserAuthDto dto)
    {
        if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
            return BadRequest(new { success = false, message = "Nincs minden mező kitöltve." });

        var user = await _authService.AuthenticateUser(dto.Username, dto.Password);
        if (user == null)
            return Unauthorized(new { success = false, message = "Hibás felhasználónév vagy jelszó." });

        string accessToken = _authService.GenerateAccessToken(user);
        string? refreshToken = user.RefreshToken;

        return Ok(new
        {
            success = true,
            accessToken,
            refreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAccessToken([FromBody] RefreshTokenDto dto)
    {
        if (string.IsNullOrEmpty(dto.RefreshToken))
            return BadRequest(new { success = false, message = "Nincs refresh token megadva." });

        var user = await _authService.CheckRefreshToken(dto.RefreshToken);
        if (user == null)
            return Unauthorized(new { success = false, message = "A refresh token érvénytelen vagy lejárt." });


        string newAccessToken = _authService.GenerateAccessToken(user);
        return Ok(new { success = true, accessToken = newAccessToken });
    }

}
