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
                if (IsGameOver(gameState))
                {
                    Console.Clear();
                    DisplayBoard(gameState.Board);
                    Console.WriteLine("A játék véget ért!");
                    break;
                }
                Console.WriteLine("Lépj!");
                Console.Write("Adj meg egy lépést honnan lépsz (x y formátumban): ");
                string input2 = Console.ReadLine();
                var parts2 = input2.Split(' ');
                Console.Write("Adj meg egy lépést (x y formátumban): ");
                string input = Console.ReadLine();
                var parts = input.Split(' ');

                if (parts.Length == 2 && int.TryParse(parts[0], out int x)
                    && int.TryParse(parts[1], out int y) && parts2.Length == 2 && int.TryParse(parts2[0], out int fromx)
                    && int.TryParse(parts2[1], out int fromy))
                {
                    if (gameState.IsValidMove(x, y, fromx, fromy))
                    {
                        gameState.MakeMove(x, y, fromx, fromy);
                        gameState.SwitchPlayer();
                    }

                }
                else
                {
                    Console.WriteLine("Érvénytelen lépés. Próbáld újra.");
                    Thread.Sleep(2000);
                }
            }
            else
            {
                Console.WriteLine("Bot lép...");
                botService.MakeBotMove();
                gameState.SwitchPlayer(); 
            }

            // A játék vége ellenőrzése (egy egyszerű logika a végén)

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
        return gameState.GeneratePossibleMoves(1).Count() == 0 || gameState.GeneratePossibleMoves(2).Count()>0;
    }
}
