namespace Backend.GameBase.Entities
{
    public record InitialGameData(int OwnPlayerId, string? OtherPlayerName, GameState State, CellState[,] Cells, int[] TimeRemaining);

    public record GameData(GameState State, CellState[,] Cells, int[] TimeRemaining);

    public record FinalGameData(GameResult GameResult, GameState State, CellState[,] Cells, int[] TimeRemaining);
}
