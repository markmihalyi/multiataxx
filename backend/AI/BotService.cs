using System.Numerics;

public class BotService
{
    private GameState _gameState;

    public BotService(GameState gameState)
    {
        _gameState = gameState;
    }

    // Bot lépés generálása
    // Jelenleg csak random (egyszerű)
    public (int x, int y, int fromx, int fromy) GenerateBotMove()
    {
        Random rand = new Random();

        var botPieces = _gameState.GetPlayerPieces(2).ToList();
        if (botPieces.Count == 0) throw new Exception("A botnak nincs elérhető lépése!");

        (int fromx, int fromy) = botPieces[rand.Next(botPieces.Count)]; // Véletlen bábu kiválasztása
        // Megkeressük az összes lehetséges célmezőt
        List<(int x, int y)> possibleMoves = new List<(int, int)>();

        int[] dx = { -1, 1, 0, 0, -1, 1, -1, 1, -2, 2, 0, 0 };
        int[] dy = { 0, 0, -1, 1, -1, 1, 1, -1, 0, 0, -2, 2 };

        for (int i = 0; i < dx.Length; i++)
        {
            int newX = fromx + dx[i];
            int newY = fromy + dy[i];

            if (newX >= 0 && newX < _gameState.Board.GetLength(0) &&
                newY >= 0 && newY < _gameState.Board.GetLength(1) &&
                _gameState.IsValidMove(newX, newY, fromx, fromy))
            {
                possibleMoves.Add((newX, newY));
            }
        }
        // Ha nincs érvényes lépés, próbálunk egy másik bábut
        if (possibleMoves.Count == 0) return GenerateBotMove();

        // Véletlenszerű érvényes célmező kiválasztása
        (int x, int y) = possibleMoves[rand.Next(possibleMoves.Count)];
        return (x, y, fromx, fromy);
    }


    // Bot lépése végrehajtása és kiírása
    public void MakeBotMove()
    {
        // Generáljuk a bot lépését
        var move = GenerateBotMove();
        int x = move.x;
        int y = move.y;
        int fromx = move.fromx;
        int fromy = move.fromy;
        // A bot lépése végrehajtása
        _gameState.MakeMove(x, y, fromx, fromy);
    }
}
