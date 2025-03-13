public class BotService
{
    private GameState _gameState;

    public BotService(GameState gameState)
    {
        _gameState = gameState;
    }

    // Bot lépés generálása
    // Jelenleg csak random (egyszerű)
    public (int x, int y) GenerateBotMove()
    {
        Random rand = new Random();
        int x, y;

        // Keressünk egy érvényes lépést
        do
        {
            x = rand.Next(0, 8); // 0 és 7 között random X pozíció
            y = rand.Next(0, 8); 
        } 
        while (_gameState.Board[x, y] != 0 || !_gameState.IsValidMove(x, y)); 
        // Ha nem üres a mező vagy érvénytelen lépés, próbálkozunk újra

        return (x, y);
    }

    // Bot lépése végrehajtása és kiírása
    public void MakeBotMove()
    {
        // Generáljuk a bot lépését
        var move = GenerateBotMove();
        int x = move.x;
        int y = move.y;
        // A bot lépése végrehajtása
        _gameState.MakeMove(x, y);
    }
}
