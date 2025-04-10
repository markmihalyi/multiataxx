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
            DisplayBoard(gameState);
            // Ha ember lép, várunk egy lépést
            if (gameState.CurrentPlayer == 1)
            {
                if (gameState.IsGameOver())
                {
                    Console.Clear();
                    DisplayBoard(gameState);
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
                    //Thread.Sleep(1000);
                }
            }
            else
            {
                Console.WriteLine("Bot lép...");
                botService.MakeBotMove();
                gameState.SwitchPlayer(); 
            }

            // TODO: A játék vége ellenőrzése (egy egyszerű logika a végén)

        }
    }

    // A tábla kirajzolása
    public static void DisplayBoard(GameState gameState)
    {
        
        for (int i = 0; i < GameState.N; i++)
        {
            for (int j = 0; j < GameState.N; j++)
            {
                char symbol = gameState.Board[i, j] == 0 ? '.' : gameState.Board[i, j] == 1 ? '1' : gameState.Board[i, j] == 2 ? '2' : 'X';
                Console.Write(symbol + " ");
            }
            Console.WriteLine();
        }
    }

}
