
using AI;
using System.Numerics;

public class BotService
{
    private GameState _gameState;
    private Minimax _minimax;
    public BotService(GameState gameState, DifficultyLevel diff)
    {
        _gameState = gameState;
        _minimax = new Minimax(gameState, (int) diff);
    }

    public (int x, int y, int fromx, int fromy) CalculateBotMove(int[,] board, int player, int size, DifficultyLevel diff)
    {
        GameState game = new GameState(board, player, size);
        Minimax minimax = new Minimax(game, (int)diff);
        var move = minimax.MaxMove(game);
        return (move.x, move.y, move.fromx, move.fromy);
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
