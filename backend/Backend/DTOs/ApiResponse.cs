namespace Backend.DTOs
{
    public record ApiResponse(bool Success, string Message);

    public record ErrorResponse(string Message) : ApiResponse(false, Message);

    public record SuccessResponse(string Message) : ApiResponse(true, Message);
}
