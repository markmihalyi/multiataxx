using System.Text.Json.Serialization;

namespace Backend.GameBase.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BoardSize
    {
        Small = 5,  // 5x5
        Medium = 7,  // 7x7
        Large = 9  // 9x9
    }

    public enum CellState
    {
        Empty = 0,
        Player1 = 1,
        Player2 = 2,
        Wall = 3
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameState
    {
        Waiting = 0,
        Player1Turn = 1,
        Player2Turn = 2,
        Ended = 3
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameResult
    {
        Draw = 0,
        Player1Won = 1,
        Player2Won = 2,
    }
}