
using AI;
using System.Numerics;
using AI.Abstractions;

public class BotService : IGameAI
{
    private GameState _gameState;
    private Minimax _minimax;
    public BotService()
    {

    }
    public BotService(GameState gameState, DifficultyLevel diff)
    {
        _gameState = gameState;
        _minimax = new Minimax(gameState, (int) diff);
    }

    public (int startX, int startY, int destX, int destY) CalculateBotMove(CellState[,] board, CellState player, BoardSize size, GameDifficulty diff)
    {
        int[,] intBoard = new int[board.GetLength(0), board.GetLength(1)];

        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                intBoard[x, y] = (int)board[x, y];
            }
        }
        GameState game = new GameState(intBoard, (int)player, (int)size);
        Minimax minimax = new Minimax(game, (int)diff);
        var move = minimax.MaxMove(game);
        return (move.fromx, move.fromy, move.x, move.y);
    }


    // Bot lépés generálása
    public (int x, int y, int fromx, int fromy) GenerateBotMove()
    {
        var move = _minimax.MaxMove(_gameState);
        return (move.x, move.y, move.fromx, move.fromy);
    }

    // Bot lépése végrehajtása és kiírása
    public void MakeBotMove()
    {
        var move = GenerateBotMove();
        // Generáljuk a bot lépését
        int x = move.x;
        int y = move.y;
        int fromx = move.fromx;
        int fromy = move.fromy;

        // A bot lépése végrehajtása
        _gameState.MakeMove(move.x, move.y, move.fromx, move.fromy);
    }
}
