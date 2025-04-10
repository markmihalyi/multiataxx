namespace Backend.DTOs
{
    public record CreateGameRequestBody(string BoardSize, double TurnMinutes);
    public record CreateGameResponse(string Message, string GameCode) : ApiResponse(true, Message);
}
