using System.Text.Json.Serialization;

namespace AI.Abstractions
{
    public enum CellState
    {
        Empty = 0,
        Player1 = 1,
        Player2 = 2,
        Wall = 3
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BoardSize
    {
        Small = 5,  // 5x5
        Medium = 7,  // 7x7
        Large = 9  // 9x9
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameDifficulty
    {
        Easy = 2,
        Medium = 3,
        Hard = 4,
    }
}
