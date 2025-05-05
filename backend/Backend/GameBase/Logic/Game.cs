using Backend.GameBase.Entities;
using Backend.Services;

namespace Backend.GameBase.Logic
{
    public class Game(GameService gameService, string GameCode, BoardSize boardSize, double turnMinutes) : IDisposable
    {
        private readonly GameService _gameService = gameService;
        public GameBoard Board { get; } = new(boardSize);

        public Player?[] Players { get; } = new Player[2];
        public int PlayerCount => Players.Count(p => p != null);
        private int FirstReadyPlayerId { get; set; } = -1;

        public GameState State { get; private set; } = GameState.Waiting;

        public TimeSpan Player1TimeRemaining { get; set; } = TimeSpan.FromMinutes(turnMinutes);
        public TimeSpan Player2TimeRemaining { get; set; } = TimeSpan.FromMinutes(turnMinutes);

        public DateTime MatchStartTimeUtc { get; set; }
        public DateTime MatchEndTimeUtc { get; set; }
        private Timer? TimeoutTimer { get; set; }

        public Player? Winner { get; private set; } = null;

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
                MatchStartTimeUtc = DateTime.UtcNow;
                TimeoutTimer = new Timer(CheckTimeout, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                var firstPlayerIndex = FirstReadyPlayerId == Players[0]?.UserId ? 0 : 1;
                await HandlePlayerTurnChange(firstPlayerIndex == 0 ? GameState.Player1Turn : GameState.Player2Turn);
            }
        }

        public async Task<string> AttemptMove(int userId, Point start, Point destination)
        {
            int playerIndex = Array.FindIndex(Players, p => p?.UserId == userId);
            if (playerIndex == -1 || playerIndex == 0 && State != GameState.Player1Turn || playerIndex == 1 && State != GameState.Player2Turn)
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

            await _gameService.NotifyGroupAsync(GameCode, "GameStateChanged", new GameData(State, Board.Cells, [(int)Player1TimeRemaining.TotalSeconds, (int)Player2TimeRemaining.TotalSeconds]));

            Debug();
        }

        private DateTime? LastTimeoutCheckedTime;

        private void CheckTimeout(object? state)
        {
            if (State != GameState.Player1Turn && State != GameState.Player2Turn) return;

            TimeSpan checkTimeDifference = (TimeSpan)(LastTimeoutCheckedTime != null ? DateTime.UtcNow - LastTimeoutCheckedTime : TimeSpan.Zero);

            if (State == GameState.Player1Turn)
            {
                Player1TimeRemaining -= checkTimeDifference;
                if (Player1TimeRemaining <= TimeSpan.Zero)
                {
                    Player1TimeRemaining = TimeSpan.Zero;
                    _ = HandleGameOver(GameResult.Player2Won);
                    TimeoutTimer?.Dispose();
                }
            }
            else if (State == GameState.Player2Turn)
            {
                Player2TimeRemaining -= checkTimeDifference;
                if (Player2TimeRemaining <= TimeSpan.Zero)
                {
                    Player2TimeRemaining = TimeSpan.Zero;
                    _ = HandleGameOver(GameResult.Player1Won);
                    TimeoutTimer?.Dispose();
                }
            }

            LastTimeoutCheckedTime = DateTime.UtcNow;
        }

        private async Task HandleGameOver(GameResult result)
        {
            MatchEndTimeUtc = DateTime.UtcNow;
            State = GameState.Ended;
            Winner = result == GameResult.Player1Won ? Players[0] : result == GameResult.Player2Won ? Players[1] : null;
            await _gameService.NotifyGroupAsync(GameCode, "GameStateChanged", new FinalGameData(result, State, Board.Cells, [(int)Player1TimeRemaining.TotalSeconds, (int)Player2TimeRemaining.TotalSeconds]));
            Console.WriteLine("Saving match data...");
            await _gameService.SaveMatchData(GameCode);
            Console.WriteLine("Match data saved!");
            await _gameService.RemoveGame(GameCode);

            Debug();
        }

        private void Debug()
        {
            Console.WriteLine($"State: {State}");
            Board.Debug();
            Console.WriteLine("Player1TimeRemaining: " + Math.Round(Player1TimeRemaining.TotalSeconds) + "s");
            Console.WriteLine("Player2TimeRemaining: " + Math.Round(Player2TimeRemaining.TotalSeconds) + "s");
            Console.WriteLine();
        }

        public void Dispose() => TimeoutTimer?.Dispose();
    }
}
