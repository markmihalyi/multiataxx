using AI.Abstractions;

namespace Backend.GameBase.Entities
{
    public record InitialSingleGameData(GameType GameType, int OwnPlayerId, string? OtherPlayerName, GameState State, CellState[,] Cells);
    public record InitialMultiGameData(GameType GameType, int OwnPlayerId, string? OtherPlayerName, GameState State, CellState[,] Cells, int[] TimeRemaining);

    public record SingleGameData(GameState State, CellState[,] Cells);
    public record MultiGameData(GameState State, CellState[,] Cells, int[] TimeRemaining);

    public record FinalSingleGameData(GameResult GameResult, GameState State, CellState[,] Cells);
    public record FinalMultiGameData(GameResult GameResult, GameState State, CellState[,] Cells, int[] TimeRemaining);
}
