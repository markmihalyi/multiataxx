using System.Text.Json.Serialization;

namespace Backend.GameBase.Entities
{
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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameType
    {
        SinglePlayer = 1,
        MultiPlayer = 2,
    }
}