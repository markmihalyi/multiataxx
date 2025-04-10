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
        /*Program.DisplayBoard(gameState);
        Thread.Sleep(1000);*/
        if (depth <= 0 || gameState.IsGameOver())
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
                //Console.WriteLine($"BOT Lépés: [{move.fromx}, {move.fromy}] -> [{move.x}, {move.y}]");
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
            return maxEval;
        }
        else
        {

            int minEval = int.MaxValue;
            gameState.SwitchPlayer();
            foreach (var move in gameState.GeneratePossibleMoves(1))
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
                //Thread.Sleep(1000);
            }
        }

        return bestMove; // Ha nem talált érvényes lépést, (-1, -1, -1, -1) értéket ad vissza
}



    // Kiértékelő


    private int Evaluate(GameState gameState)
    {
        int botScore = gameState.GetPlayerPiecesCount(2);
        int playerScore = gameState.GetPlayerPiecesCount(1);

        // Pozíció-alapú értékelés
        int botPositionAdvantage = 0;
        foreach (var (x, y) in gameState.GetPlayerPieces(2))
        {
            if (x >= 3 && x <= 5 && y >= 3 && y <= 5) // középső sáv
            {
                botPositionAdvantage += 3; // Több pont a középső mezőn lévő bábukért
            }
        }

        int playerPositionAdvantage = 0;
        foreach (var (x, y) in gameState.GetPlayerPieces(1))
        {
            if (x >= 3 && x <= 5 && y >= 3 && y <= 5)
            {
                playerPositionAdvantage += 3;
            }
        }
        
        // Mozgási lehetőségek (mobilitás)
        int botMoves = gameState.GeneratePossibleMovesCount(2);
        int playerMoves = gameState.GeneratePossibleMovesCount(1);

        // Súlyozott értékelés
        return (10 * botScore + 5 * botPositionAdvantage + 2 * botMoves) -
               (10 * playerScore + 5 * playerPositionAdvantage + 2 * playerMoves);
    }

}
