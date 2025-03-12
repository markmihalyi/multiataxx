using System;

class Program
{
    static void Main(string[] args)
    {
        // inicializálás
        GameState gameState = new GameState();
        BotService botService = new BotService(gameState);

        // A játék futtatása
        while (true)
        {
            Console.Clear();
            DisplayBoard(gameState.Board);

            // Ha ember lép, várunk egy lépést
            if (gameState.CurrentPlayer == 1)
            {
                Console.WriteLine("Lépj!");
                Console.Write("Adj meg egy lépést (x y formátumban): ");
                string input = Console.ReadLine();
                var parts = input.Split(' ');

                if (parts.Length == 2 && int.TryParse(parts[0], out int x)
                    && int.TryParse(parts[1], out int y))
                {
                    gameState.MakeMove(x, y);
                }
                else
                {
                    Console.WriteLine("Érvénytelen lépés. Próbáld újra.");
                }
            }
            else
            {
                Console.WriteLine("Bot lép...");
                botService.MakeBotMove();
            }

            // A játék vége ellenőrzése (egy egyszerű logika a végén)
            if (IsGameOver(gameState))
            {
                Console.Clear();
                DisplayBoard(gameState.Board);
                Console.WriteLine("A játék véget ért!");
                break;
            }
        }
    }

    // A tábla kirajzolása
    static void DisplayBoard(int[,] board)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                char symbol = board[i, j] == 0 ? '.' : board[i, j] == 1 ? '1' : '2';
                Console.Write(symbol + " ");
            }
            Console.WriteLine();
        }
    }
    // Tábla tele van akkor vége
    static bool IsGameOver(GameState gameState)
    {
        foreach (var cell in gameState.Board)
        {
            if (cell == 0) return false; // Ha van üres mező, még nem ért véget a játék
        }
        return true;
    }
}
