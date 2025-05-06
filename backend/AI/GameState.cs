using System.Drawing;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using AI.Abstractions;

public class GameState
{
    public int[,] Board { get; private set; }
    public static int N; // Tábla mérete
    public int CurrentPlayer { get; private set; }
    private static int[] dx = { -1, 1, 0, 0, -1, 1, -1, 1, -2, 2, 0, 0, 2, -2, -2, 2 };
    private static int[] dy = { 0, 0, -1, 1, -1, 1, 1, -1, 0, 0, -2, 2, 2, -2, 2, -2 };

    private Stack<(int fromx, int fromy, int tox, int toy)> moveHistory
    = new Stack<(int, int, int, int)>();

    public GameState(int boardSize)
    {
        N = boardSize;
        CurrentPlayer = 1; // Kezdetben Player 1 lép (azaz az ember)
        InitializeBoard();
    }
    public GameState(int[,] board, int currentPlayer, int boardSize)
    {
        Board = board;
        CurrentPlayer = currentPlayer;
        N = boardSize;
        InitializeBoard();
    }

    // Kezdő pozíciók beállítása
    public void InitializeBoard()
    {
        Board = new int[N, N];

        // Játékosok kezdőpozíciója
        Board[0, 0] = 1; // Player 1
        Board[N - 1, N - 1] = 2; // Player 2
        int mid = N / 2;
        int size = (N >= 7) ? ((N - 1) / 2) | 1 : 3; // 3x3, 5x5, 7x7 stb.
        Board[N / 2, N / 2] = 3; // Akadály
        if(N >= 7)
        {
            Board[N / 2 - 1, N / 2 - 1] = 3;
            Board[N / 2 + 1, N / 2 - 1] = 3;
            Board[N / 2 - 1, N / 2 + 1] = 3;
            Board[N / 2 + 1, N / 2 + 1] = 3;

        }
        if(N >= 9)
        {
            Board[N / 2 - 2, N / 2 - 2] = 3;
            Board[N / 2 + 2, N / 2 - 2] = 3;
            Board[N / 2 - 2, N / 2 + 2] = 3;
            Board[N / 2 + 2, N / 2 + 2] = 3;
        }
    }

    // Játékos váltás
    public void SwitchPlayer()
    {
        CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
    }


    // Érvényes lépés ellenőrzése
    public bool IsValidMove(int x, int y, int fromx, int fromy)
    {
        if (!IsValidCoordinate(x, y))
        {
            return false;
        }
        if (!IsValidCoordinate(fromx, fromy))
        {
            return false;
        }
        if (Board[x, y] != 0 )
        {
            return false;
        }

        if (Board[fromx, fromy] != CurrentPlayer)
        {
            return false;
        }

        if (Math.Abs(x - fromx) <= 2 && Math.Abs(y - fromy) <= 2)
        {
                return true;
        }

        // Ha nem találtunk a közelben saját bábút, akkor érvénytelen lépés
        return false;

    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsValidCoordinate(int x, int y)
    {
        return (uint)x < N && (uint)y < N;
    }

    // Lépés végrehajtása és a környező mezők frissítése
    public void MakeMove(int x, int y, int fromx, int fromy)
    {
        if (IsValidMove(x, y, fromx, fromy))
        {
            moveHistory.Push((fromx, fromy, x, y));
            Board[x, y] = CurrentPlayer;
            JumpUpdateCells(x, y, fromx, fromy);
            UpdateSurroundingCells(x, y); // Frissítjük a környező mezőket
        }
        else
        {
            Console.WriteLine($"Lépés: {x} {y} {fromx} {fromy}");
            Console.WriteLine("Érvénytelen lépés!");
        }
    }
    private void JumpUpdateCells(int x, int y, int fromx, int fromy)
    {
        // Ha ugrunk akkor az előző mezőt töröljük
        if (Math.Abs(x - fromx) == 2 && Math.Abs(y - fromy) == 2
            ||
            Math.Abs(x - fromx) == 0 && Math.Abs(y - fromy) == 2
            ||
            Math.Abs(x - fromx) == 2 && Math.Abs(y - fromy) == 0
            )
        {
            Board[fromx, fromy] = 0;
        }
    }
    // Környező mezők frissítése
    private void UpdateSurroundingCells(int x, int y)
    {
        int[] dx = { -1, 1, 0, 0, 1, -1, 1, -1};
        int[] dy = { 0, 0, -1, 1, 1, -1, -1, 1};

        for (int dir = 0; dir < dx.Length; dir++)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];

            if (nx >= 0 && nx < N && ny >= 0 && ny < N)
            {
                if (Board[nx, ny] != 0 && Board[nx, ny] !=3)
                {
                    // Itt frissítjük a mezőt a jelenlegi játékos bábujára
                    Board[nx, ny] = CurrentPlayer;
                }
            }
        }
    }

    // Tábla kiírása
    public void DisplayBoard()
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                char c = Board[i, j] == 0 ? '.' :
                         Board[i, j] == 1 ? '1' : '2';
                Console.Write(c + " ");
            }
            Console.WriteLine();
        }
    }

    // Bábuk helyeinek visszaadása
    public IEnumerable<(int, int)> GetPlayerPieces(int player)
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (Board[i, j] == player)
                {
                    yield return (i, j);
                }
            }
        }
    }
    public int GetPlayerPiecesCount(int player)
    {
        int count = 0;
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (Board[i, j] == player)
                {
                    count++;
                }
            }
        }
        return count;
    }
    // Utolsó lépés visszavonása
    public void UndoLastMove()
    {
        if (moveHistory.Count > 0)
        {
            var lastMove = moveHistory.Pop();
            Board[lastMove.fromx, lastMove.fromy] = CurrentPlayer;
            Board[lastMove.tox, lastMove.toy] = 0;
        }
    }
    // Játékállapot klónozása
    public GameState Clone()
    {
        GameState newState = new GameState(N);

        // Másolat készítése a játékállapotról
        newState.Board = (int[,])this.Board.Clone();
        newState.CurrentPlayer = this.CurrentPlayer;
        newState.moveHistory = new Stack<(int fromx, int fromy, int tox, int toy)>
            (this.moveHistory.Select(item => (item.fromx, item.fromy, item.tox, item.toy)));

        return newState;
    }
    public int GeneratePossibleMovesCount(int player)
    {
        bool valtott = false;
        if (player != CurrentPlayer)
        {
            valtott = true;
            SwitchPlayer();
        }

        int count = 0;
        var playerPieces = GetPlayerPieces(player);
        foreach (var (fromx, fromy) in playerPieces)
        {
            for (int i = 0; i < dx.Length; i++)
            {
                int newX = fromx + dx[i];
                int newY = fromy + dy[i];

                if (IsValidMove(newX, newY, fromx, fromy))
                {
                    count++;
                }
            }
        }
        if (valtott)
        {
            SwitchPlayer();
        }
        return count;
    }
    public List<(int x, int y, int fromx, int fromy)> GeneratePossibleMoves( int player)
    {
        bool valtott = false;
        if(player != CurrentPlayer)
        {
            valtott = true;
            SwitchPlayer();
        }
        List<(int x, int y, int fromx, int fromy)> possibleMoves = new List<(int, int, int, int)>();

        var playerPieces = GetPlayerPieces(player);
        foreach (var (fromx, fromy) in playerPieces)
        {
            for (int i = 0; i < dx.Length; i++)
            {
                int newX = fromx + dx[i];
                int newY = fromy + dy[i];

                if (IsValidMove(newX, newY, fromx, fromy))
                {
                    possibleMoves.Add((newX, newY, fromx, fromy));
                }
            }
        }
        if (valtott)
        {
            SwitchPlayer();
        }
        return possibleMoves;
    }
    public void Fill(int player)
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (Board[i, j] == 0)
                {
                    Board[i, j] = player;
                }
            }
        }
    }
    // Tábla tele van akkor vége
    public bool IsGameOver()
    {
        if(GeneratePossibleMoves(1).Count == 0)
        {
            Fill(2);

            return true;
        }
        else if (GeneratePossibleMoves(2).Count == 0)
        {
            Fill(1);
            return true;
        }
            return false;
    }
    // Üres helyek feltöltése a táblán

}
