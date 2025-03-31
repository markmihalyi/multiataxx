using Backend.GameLogic.Entities;
using Backend.Services;

namespace Backend.GameLogic.Logic
{
    public class Game(GameService gameService, string gameCode, BoardSize boardSize)
    {
        private readonly GameService _gameService = gameService;

        public string GameCode { get; } = gameCode;

        public GameBoard Board { get; } = new(boardSize);

        public Player?[] Players { get; } = new Player[2];

        public int PlayerCount => Players.Count(p => p != null);

        public GameState State { get; private set; } = GameState.Waiting;

        public Player? Winner { get; private set; } = null;

        private int FirstReadyPlayerId { get; set; } = -1;

        public bool TryJoin(Player player)
        {
            if (Players.Any(p => p?.UserId == player.UserId))
            {
                return true;
            }

            if (PlayerCount < 2)
            {
                Players[PlayerCount] = player;
                return true;
            }

            return false;
        }

        public async Task SetPlayerIsReady(int userId)
        {
            int playerIndex = Array.FindIndex(Players, p => p?.UserId == userId);
            if (playerIndex == -1) return;

            var player = Players[playerIndex];
            if (player == null || player.IsReady) return;

            player.IsReady = true;

            if (FirstReadyPlayerId == -1)
            {
                FirstReadyPlayerId = userId;
            }

            int otherPlayerIndex = 1 - playerIndex;
            bool otherPlayerIsReady = Players[otherPlayerIndex]?.IsReady ?? false;

            if (PlayerCount == 2 && otherPlayerIsReady)
            {
                var firstPlayerIndex = FirstReadyPlayerId == Players[0]?.UserId ? 0 : 1;
                await HandlePlayerTurnChange(firstPlayerIndex == 0 ? GameState.Player1Turn : GameState.Player2Turn);
            }
        }

        public async Task<string> AttemptMove(int userId, Point start, Point destination)
        {
            int playerIndex = Array.FindIndex(Players, p => p?.UserId == userId);
            if (playerIndex == -1 || (playerIndex == 0 && State != GameState.Player1Turn) || (playerIndex == 1 && State != GameState.Player2Turn))
            {
                return "NotYourTurn";
            }

            CellState ownCellState = playerIndex == 0 ? CellState.Player1 : CellState.Player2;
            MoveType moveType = MoveValidator.ValidateMove(start, destination, Board.Cells, ownCellState);
            if (moveType == MoveType.INVALID)
            {
                return "InvalidMoveAttempted";
            }

            Board.PerformMove(start, destination, moveType, ownCellState);

            var (isGameOver, gameResult) = Board.CheckIfGameIsOver();
            if (isGameOver && gameResult.HasValue)
            {
                await HandleGameOver(gameResult.Value);
            }
            else
            {
                await HandlePlayerTurnChange(playerIndex == 0 ? GameState.Player2Turn : GameState.Player1Turn);
            }

            return "MoveCompleted";
        }

        private async Task HandlePlayerTurnChange(GameState state)
        {
            State = state;

            try
            {
                await _gameService.NotifyGroupAsync(GameCode, "GameStateChanged", new GameData(State, Board.Cells));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NotifyGroupAsync: {ex.Message}");
            }

            // -> DEBUG
            Console.WriteLine($"State: {State}");
            Board.Debug();
            Console.WriteLine();
            // <- DEBUG
        }

        private async Task HandleGameOver(GameResult result)
        {
            State = GameState.Ended;

            try
            {
                await _gameService.NotifyGroupAsync(GameCode, "GameStateChanged", new FinalGameData(result, State, Board.Cells));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in NotifyGroupAsync: {ex.Message}");
            }

            await _gameService.RemoveGame(GameCode);

            // -> DEBUG
            Console.WriteLine($"State: {State}");
            Board.Debug();
            Console.WriteLine();
            // <- DEBUG
        }
    }
}
