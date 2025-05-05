using Backend.GameBase.Entities;
using System.Text.Json.Serialization;

namespace Backend.DTOs
{
    public record CreateGameRequestBody(string BoardSize, double TurnMinutes);
    public record CreateGameResponse(string Message, string GameCode) : ApiResponse(true, Message);
    public record MatchHistoryData(Guid Id, MatchResult Result, int Duration, DateTime Date);
    public record MatchDetails(Guid Id, MatchResult Result, List<string> Players, List<CellState[,]> Steps);

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MatchResult
    {
        Won = 1,
        Draw = 2,
        Lost = 3,
    }
}
