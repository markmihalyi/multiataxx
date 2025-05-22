namespace AI.Abstractions
{
    public interface IGameAI
    {
        /// <summary>
        /// Calculates the bot's next move based on current game board state.
        /// </summary>
        /// <param name="board">2D array representing the game board where each cell has a CellState value</param>
        /// <param name="botCellState">Specifies which CellState value represents the bot's cells</param>
        /// <param name="size">Size of the game board</param>
        /// <param name="difficulty">Bot difficulty</param>
        /// <returns>Tuple containing move coordinates: (startX, startY, destX, destY)</returns>
        (int startX, int startY, int destX, int destY) CalculateBotMove(CellState[,] board, CellState botCellState, BoardSize size, GameDifficulty difficulty);
    }
}
