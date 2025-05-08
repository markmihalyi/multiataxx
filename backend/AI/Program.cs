using System;
using System.Drawing;
using AI.Abstractions;

class Program
{
    static void Main(string[] args)
    {
        // inicializálás
        GameState gameState = new GameState(5);
        BotService botService = new BotService(gameState, AI.DifficultyLevel.Hard);

        // első oszlop 2. sor
        // A játék futtatása
        while (true)
        {
            Console.Clear();
            DisplayBoard(gameState);
            /*
            CellState[,] cells = new CellState[(int)BoardSize.Small, (int)BoardSize.Small]
            {
                { CellState.Player1, CellState.Empty,   CellState.Empty, CellState.Empty,   CellState.Empty },
                { CellState.Empty,   CellState.Empty,   CellState.Empty, CellState.Empty,   CellState.Empty },
                { CellState.Empty,   CellState.Empty,   CellState.Wall,  CellState.Empty,   CellState.Empty },
                { CellState.Empty,   CellState.Empty,   CellState.Empty, CellState.Empty,   CellState.Empty },
                { CellState.Empty,   CellState.Empty,   CellState.Empty, CellState.Empty,   CellState.Player1 }
            };
            BotService botservice1 = new BotService();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Console.Write((int)cells[i,j]);
                }
                Console.WriteLine();
            }
            var move = botservice1.CalculateBotMove(cells, CellState.Player1, BoardSize.Small, GameDifficulty.Hard);
            var move2 = botservice1.CalculateBotMove(cells, CellState.Player2, BoardSize.Small, GameDifficulty.Hard);

            Console.WriteLine($"1es player: {move.startX} {move.startY} --->  {move.destX} {move.destY}");
            Console.WriteLine($"2es player: {move2.startX} {move2.startY} --->  {move2.destX} {move2.destY}");

            Thread.Sleep(5000);*/


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
                }
            }
            else
            {
                Console.WriteLine("Bot lép...");
                botService.MakeBotMove();
                gameState.SwitchPlayer(); 
            }
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
