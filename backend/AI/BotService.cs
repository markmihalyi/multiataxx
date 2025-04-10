
using System.Numerics;

public class BotService
{
    private GameState _gameState;
    private Minimax _minimax;
    public BotService(GameState gameState)
    {
        _gameState = gameState;
        _minimax = new Minimax(gameState, 5);
    }

    // Bot lépés generálása
    // Jelenleg csak random (egyszerű)
    public (int x, int y, int fromx, int fromy) GenerateBotMove()
    {
        // TODO: Booster metódus implementálása

        return (0, 0, 0, 0);
    }

    
    // Bot lépése végrehajtása és kiírása
    public void MakeBotMove()
    {
        var move = _minimax.MaxMove(_gameState);
        // Generáljuk a bot lépését
        int x = move.x;
        int y = move.y;
        int fromx = move.fromx;
        int fromy = move.fromy;
        // A bot lépése végrehajtása
        // Ha a bot nem talált érvényes lépést
        if (x == -1 && y == -1 && fromx == -1 && fromy == -1)
        {
            Console.WriteLine("A bot nem tudott lépni!");
            return;
        }

        // A bot lépése végrehajtása
        _gameState.MakeMove(x, y, fromx, fromy);
        Console.WriteLine($"Bot moved from actually ({fromx}, {fromy}) to ({x}, {y})");
        //Thread.Sleep(2000);
    }

}
