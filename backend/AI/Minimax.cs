using System.Text.Json;


public class Minimax
{
    private GameState _gameState;
    private int _maxDepth;
    string memoryFilePath = "memory.json";
    private int maxPlayer = 2;
    private int minPlayer = 1;


    private Dictionary<string, int> _memoria = new Dictionary<string, int>();
    public Minimax(GameState gameState, int maxDepth, int maximazingPlayer = 2)
    {
        maxPlayer = maximazingPlayer;
        minPlayer = (maxPlayer == 2) ? 1 : 2;

        _gameState = gameState;
        _maxDepth = maxDepth;

        if (File.Exists(memoryFilePath))
        {
            LoadMemoryFromFile(memoryFilePath);
        }
    }
    public void SaveMemoryToFile(string filePath)
    {
        var json = JsonSerializer.Serialize(_memoria, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public void LoadMemoryFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            _memoria = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? [];
        }
        else
        {
            _memoria = [];
        }
    }

    private string GetGameStateKey(GameState gameState)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        // A tábla adatait és a játékos azonosítóját egy byte tömbbe másoljuk
        var boardBytes = new byte[gameState.Board.Length * sizeof(int) + 3];
        Buffer.BlockCopy(gameState.Board, 0, boardBytes, 0, gameState.Board.Length * sizeof(int));
        boardBytes[^1] = (byte)gameState.CurrentPlayer; // Játékos azonosító hozzáadása
        boardBytes[^2] = (byte)maxPlayer; // Játékos azonosító hozzáadása
        boardBytes[^3] = (byte)_maxDepth; // Játékos azonosító hozzáadása

        // Hash kiszámítása
        var hash = sha256.ComputeHash(boardBytes);
        // Base64 stringként visszaadjuk a kulcsot
        return Convert.ToBase64String(hash);
    }


    public int MinimaxAlgorithm(GameState gameState, int depth, bool isMaxPlayer, int alpha, int beta)
    {
        string key = GetGameStateKey(gameState);
        if (depth <= 0 || gameState.IsGameOver())
        {
            return Evaluate(gameState);
        }
        // Ellenőrizzük, hogy az állapot már szerepel-e a memóban
        if (_memoria.ContainsKey(key))
        {

            return _memoria[key];  // Visszaadjuk a memóban tárolt értéket
        }

        if (isMaxPlayer)
        {

            // Maximalizálni kívánt ág
            int maxEval = int.MinValue;
            gameState.SwitchPlayer();
            foreach (var move in gameState.GeneratePossibleMoves(maxPlayer))
            {
                GameState newState = gameState.Clone();
                newState.MakeMove(move.x, move.y, move.fromx, move.fromy);
                int eval = MinimaxAlgorithm(newState, depth - 1, !isMaxPlayer, alpha, beta);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);

                if (beta <= alpha)
                {
                    break;  // Metszés
                }

            }
            _memoria[key] = maxEval;  // Tároljuk az értéket
            return maxEval;
        }
        else
        {

            int minEval = int.MaxValue;
            gameState.SwitchPlayer();
            foreach (var move in gameState.GeneratePossibleMoves(minPlayer))
            {
                //Console.WriteLine($"Játékos Lépés: [{move.fromx}, {move.fromy}] -> [{move.x}, {move.y}]");
                GameState newState = gameState.Clone();
                newState.MakeMove(move.x, move.y, move.fromx, move.fromy);
                int eval = MinimaxAlgorithm(newState, depth - 1, !isMaxPlayer, alpha, beta);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);  // Frissítjük a beta értéket

                if (beta <= alpha)
                {
                    break;  // Metszés
                }

            }
            _memoria[key] = minEval;  // Tároljuk az értéket
            return minEval;
        }
    }
    public (int x, int y, int fromx, int fromy) MaxMove(GameState gameState)
    {
        (int x, int y, int fromx, int fromy) bestMove = (-1, -1, -1, -1); // Hibás lépés alapértéke
        int maxEval = int.MinValue; // - végtelen
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        foreach (var move in gameState.GeneratePossibleMoves(maxPlayer))
        {
            GameState newState = gameState.Clone();
            newState.MakeMove(move.x, move.y, move.fromx, move.fromy);
            int eval = MinimaxAlgorithm(newState, _maxDepth - 1, false, alpha, beta);
            if (eval > maxEval)
            {
                maxEval = eval;
                bestMove = move;
            }
        }
        SaveMemoryToFile(memoryFilePath);
        return bestMove; // Ha nem talált érvényes lépést, (-1, -1, -1, -1) értéket ad vissza
    }



    // Kiértékelő


    private int Evaluate(GameState gameState)
    {
        int botScore = gameState.GetPlayerPiecesCount(maxPlayer);
        int playerScore = gameState.GetPlayerPiecesCount(minPlayer);

        // Pozíció-alapú értékelés
        int botPositionAdvantage = 0;
        foreach (var (x, y) in gameState.GetPlayerPieces(maxPlayer))
        {
            if (x >= 3 && x <= 5 && y >= 3 && y <= 5) // középső sáv
            {
                botPositionAdvantage += 3; // Több pont a középső mezőn lévő bábukért
            }
        }

        int playerPositionAdvantage = 0;
        foreach (var (x, y) in gameState.GetPlayerPieces(minPlayer))
        {
            if (x >= 3 && x <= 5 && y >= 3 && y <= 5)
            {
                playerPositionAdvantage += 3;
            }
        }

        // Mozgási lehetőségek (mobilitás)
        int botMoves = gameState.GeneratePossibleMovesCount(maxPlayer);
        int playerMoves = gameState.GeneratePossibleMovesCount(minPlayer);

        // Súlyozott értékelés
        return (10 * botScore + 5 * botPositionAdvantage + 2 * botMoves) -
               (10 * playerScore + 5 * playerPositionAdvantage + 2 * playerMoves);
    }

}
