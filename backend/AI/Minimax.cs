using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;

public class Minimax
{
    private GameState _gameState; 
    private int _maxDepth;

    public Minimax(GameState gameState, int maxDepth)
    {
        _gameState = gameState; 
        _maxDepth = maxDepth;
    }

    public int MinimaxAlgorithm(GameState gameState, int depth, bool isMaxPlayer, int alpha, int beta)
    {
        if (depth <= 0 || GameOver(gameState))
        {
            return Evaluate(gameState);
        }

        if (isMaxPlayer)
        {

            // Maximalizálni kívánt ág
            int maxEval = int.MinValue;
            gameState.SwitchPlayer();
            foreach (var move in gameState.GeneratePossibleMoves(2))
            {
                Console.WriteLine($"BOT Lépés: [{move.fromx}, {move.fromy}] -> [{move.x}, {move.y}]");
                GameState newState = gameState.Clone();
                newState.MakeMove(move.x, move.y, move.fromx, move.fromy);
                int eval = MinimaxAlgorithm(newState, depth - 1, !isMaxPlayer, alpha, beta);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);

                if (beta <= alpha)
                {
                    Console.WriteLine("KILÉP ALPHA");
                    break;  // Metszés
                }

            }
            return maxEval;
        }
        else
        {

            int minEval = int.MaxValue;
            gameState.SwitchPlayer();
            foreach (var move in gameState.GeneratePossibleMoves(1))
            {
                Console.WriteLine($"Játékos Lépés: [{move.fromx}, {move.fromy}] -> [{move.x}, {move.y}]");
                GameState newState = gameState.Clone();
                newState.MakeMove(move.x, move.y, move.fromx, move.fromy);
                int eval = MinimaxAlgorithm(newState, depth - 1, !isMaxPlayer, alpha, beta);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);  // Frissítjük a beta értéket

                if (beta <= alpha)
                {
                    Console.WriteLine("KILÉP BETA");
                    break;  // Metszés
                }

            }
            return minEval;
        }
    }
    public (int x, int y, int fromx, int fromy) MaxMove(GameState gameState)
    {
        (int x, int y, int fromx, int fromy) bestMove = (-1, -1, -1, -1); // Hibás lépés alapértéke
        int maxEval = int.MinValue; // - végtelen
        int alpha = int.MinValue;
        int beta = int.MaxValue;
        foreach (var move in gameState.GeneratePossibleMoves(2))
        {
            GameState newState = gameState.Clone(); 
            newState.MakeMove(move.x, move.y, move.fromx, move.fromy);
            int eval = MinimaxAlgorithm(newState, _maxDepth - 1, false, alpha, beta);
            if (eval > maxEval)
            {
                maxEval = eval;
                bestMove = move;
                Console.WriteLine($"Frissített maxEval: {maxEval}, Lépés: [{move.fromx}, {move.fromy}] -> [{move.x}, {move.y}]");
                Thread.Sleep(2000);
            }
        }

        return bestMove; // Ha nem talált érvényes lépést, (-1, -1, -1, -1) értéket ad vissza
}


    public Boolean GameOver(GameState gameState)
    {
        if (gameState.GeneratePossibleMoves(1).Count() == 0 || gameState.GeneratePossibleMoves(2).Count() == 0)
        {
            Console.WriteLine("Game Over állapot elérve!");
            return true;
        }
        return false;
    }
    // Lehetséges lépések generálása
   


    // Kiértékelő
    private int Evaluate(GameState gameState)
    {
        // TODO: Jobb kiértékelő függvény 
        int botScore = gameState.GetPlayerPieces(2).Count();
        int playerScore = gameState.GetPlayerPieces(1).Count();

        return botScore - playerScore;
    }

}
