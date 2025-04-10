namespace Backend.DTOs
{
    public record CreateGameRequestBody(string BoardSize);
    public record CreateGameResponse(string Message, string GameCode) : ApiResponse(true, Message);
}
