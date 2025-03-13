namespace Backend.DTOs
{
    public record AuthRequestBody(string Username, string Password);

    public record LoginResponse(string Message, string AccessToken, string RefreshToken) : ApiResponse(true, Message);

    public record TokenRequestBody(string RefreshToken);

    public record TokenResponse(string Message, string AccessToken) : ApiResponse(true, Message);
}
