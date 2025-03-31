namespace Backend.GameLogic.Entities
{
    public record InitialGameData(int OwnPlayerId, string? OtherPlayerName, GameState State, CellState[,] Cells);

    public record GameData(GameState State, CellState[,] Cells);

    public record FinalGameData(GameResult GameResult, GameState State, CellState[,] Cells);
}
