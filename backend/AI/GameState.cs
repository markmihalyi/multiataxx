public class GameState
{
    // Ez az osztály csak azért van, hogy ameddig nincs backend amin megtudom jeleníteni addig tudjam tesztelni.
    // Később összelesz fésülve a backendel.
    public int[,] Board { get; private set; }
    public int CurrentPlayer { get; private set; }

    public GameState()
    {
        Board = new int[8, 8];
        InitializeBoard();
        CurrentPlayer = 1; // Kezdetben Player 1 lép (azaz az ember)
    }

    // Kezdő pozíciók beállítása
    public void InitializeBoard()
    {
        Board[0, 0] = 1; // Player 1 bábujának kezdő pozíciója (bal felső sarok)

        Board[7, 7] = 2; // Player 2 bábujának kezdő pozíciója (jobb alsó sarok)
    }

    // Játékos váltás
    public void SwitchPlayer()
    {
        CurrentPlayer = (CurrentPlayer == 1) ? 2 : 1;
    }

    // Érvényes lépés ellenőrzése
    public bool IsValidMove(int x, int y)
    {
        int[] dx = { -1, 1, 0, 0, -1, 1, -1, 1, -2, 2, 0, 0 };
        int[] dy = { 0, 0, -1, 1, -1, 1, 1, -1, 0, 0, -2, 2 };

        // Ellenőrizzük, hogy az aktuális mező üres legyen
        if (Board[x, y] != 0)
        {
            return false; // Nem érvényes, ha már van ott bábu
        }

        for (int i = 0; i < dx.Length; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && nx < Board.GetLength(0) && ny >= 0 && ny < Board.GetLength(1))
            {
                // Ha a környező mezőn saját bábu található
                if (Board[nx, ny] == CurrentPlayer)
                {
                    return true;
                }
            }
        }

        // Ha nem találtunk a közelben saját bábút, akkor érvénytelen lépés
        return false;

    }

    // Lépés végrehajtása és a környező mezők frissítése
    //Nem kezeli, hogy ha ugrik valaki törölje az előző mezőt. Mert nem tudja melyikről lépnek.
    public void MakeMove(int x, int y)
    {
        if (IsValidMove(x, y))
        {
            Board[x, y] = CurrentPlayer;
            UpdateSurroundingCells(x, y); // Frissítjük a környező mezőket
            SwitchPlayer(); // Váltás a következő játékosra
        }
        else
        {
            Console.WriteLine("Érvénytelen lépés!");
        }
    }

    // Környező mezők frissítése
    private void UpdateSurroundingCells(int x, int y)
    {
        int[] dx = { -1, 1, 0, 0, 1, -1, 1, -1};
        int[] dy = { 0, 0, -1, 1, 1, -1, -1, 1};

        for (int dir = 0; dir < 4; dir++)
        {
            int nx = x + dx[dir];
            int ny = y + dy[dir];

            if (nx >= 0 && nx < Board.GetLength(0) && ny >= 0 && ny < Board.GetLength(1))
            {
                if (Board[nx, ny] != 0)
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
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                char c = Board[i, j] == 0 ? '.' :
                         Board[i, j] == 1 ? '1' : '2';
                Console.Write(c + " ");
            }
            Console.WriteLine();
        }
    }
}
