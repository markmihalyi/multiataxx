namespace Backend.DTOs
{
    public record AuthRequestBody(string Username, string Password);
    public record UserDetailsResponse(int Id, string Username);
}
