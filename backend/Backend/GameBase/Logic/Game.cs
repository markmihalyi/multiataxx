using AI.Abstractions;
using Backend.GameBase.Entities;
using Backend.Services;

namespace Backend.GameBase.Logic
{
    public class Game : IDisposable
    {
        private readonly GameService _gameService;
        private readonly IGameAI? _gameAI;

        public string GameCode { get; }
        public GameDifficulty Difficulty { get; }

        public GameBoard Board { get; }
        public Player?[] Players { get; } = new Player[2];
        public int PlayerCount => Players.Count(p => p != null);
        private int FirstReadyPlayerId { get; set; } = -1;

        public GameState State { get; private set; } = GameState.Waiting;

        private TimeSpan Player1TimeRemaining { get; set; }
        private TimeSpan Player2TimeRemaining { get; set; }
        public int[] RemainingTimes => [(int)Player1TimeRemaining.TotalSeconds, (int)Player2TimeRemaining.TotalSeconds];

        public DateTime MatchStartTimeUtc { get; set; }
        public DateTime MatchEndTimeUtc { get; set; }
        private Timer? TimeoutTimer { get; set; }

        public Player? Winner { get; private set; } = null;
        public GameType Type { get; }

        public Game(IServiceProvider provider, string gameCode, GameType gameType, BoardSize boardSize, double turnMinutes)
        {
            _gameService = provider.GetRequiredService<GameService>();
            GameCode = gameCode;
            Type = gameType;
            Board = new(boardSize);
            Player1TimeRemaining = TimeSpan.FromMinutes((double)turnMinutes);
            Player2TimeRemaining = TimeSpan.FromMinutes((double)turnMinutes);
        }

        public Game(IServiceProvider provider, string gameCode, GameType gameType, BoardSize boardSize, GameDifficulty difficulty)
        {
            _gameService = provider.GetRequiredService<GameService>();
            _gameAI = provider.GetRequiredService<IGameAI>();
            GameCode = gameCode;
            Type = gameType;
            Board = new(boardSize);
            Difficulty = difficulty;
        }

        public async Task<bool> TryJoin(Player player)
        {
            if (Type == GameType.MultiPlayer)
            {
                if (Players.Any(p => p?.UserId == player.UserId))
                {
                    return true;
                }

                if (PlayerCount < 2)
                {
                    Players[PlayerCount] = player;
                    await _gameService.NotifyGroupAsync(GameCode, "PlayerJoined", player.Name);
                    return true;
                }
            }
            else if (Type == GameType.SinglePlayer)
            {
                if (PlayerCount < 2)
                {
                    Players[0] = player;
                    Players[1] = new Player(null, null, "Bot") { IsReady = true };

                    // Start single player game
                    MatchStartTimeUtc = DateTime.UtcNow;
                    await HandlePlayerTurnChange(GameState.Player1Turn);
                    return true;
                }
            }

            return false;
        }

        // Only available in multiplayer
        public async Task SetPlayerIsReady(int userId)
        {
            if (State != GameState.Waiting || Type != GameType.MultiPlayer) return;

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
                // Start multiplayer game
                MatchStartTimeUtc = DateTime.UtcNow;
                TimeoutTimer = new Timer(CheckTimeout, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                var firstPlayerIndex = FirstReadyPlayerId == Players[0]?.UserId ? 0 : 1;
                await HandlePlayerTurnChange(firstPlayerIndex == 0 ? GameState.Player1Turn : GameState.Player2Turn);
            }
        }

        public async Task<string> AttemptMove(int? userId, Point start, Point destination, bool isBot = false)
        {
            int playerIndex = userId != null ? Array.FindIndex(Players, p => p?.UserId == userId) : !isBot ? 0 : 1;
            if ((playerIndex == -1 || playerIndex == 0 && State != GameState.Player1Turn || playerIndex == 1 && State != GameState.Player2Turn) && !isBot)
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

            if (Type == GameType.MultiPlayer)
            {
                Debug();
                await _gameService.NotifyGroupAsync(GameCode, "GameStateChanged", new MultiGameData(State, Board.Cells, RemainingTimes));
            }
            else if (Type == GameType.SinglePlayer)
            {
                await _gameService.NotifyGroupAsync(GameCode, "GameStateChanged", new SingleGameData(State, Board.Cells));

                if (state == GameState.Player2Turn)
                {
                    // AI move
                    Debug();
                    var move = (_gameAI?.CalculateBotMove(Board.Cells, CellState.Player2, (BoardSize)Board.Size, Difficulty)) ?? throw new Exception("AI not responding.");
                    await Task.Delay(1000);
                    await AttemptMove(null, new Point(move.startX, move.startY), new Point(move.destX, move.destY), true);
                    Console.WriteLine("AI move: " + move.ToString());
                    Debug();
                }
            }
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

            if (Type == GameType.MultiPlayer)
            {
                await _gameService.NotifyGroupAsync(GameCode, "GameStateChanged", new FinalMultiGameData(result, State, Board.Cells, RemainingTimes));
                var matchData = await _gameService.SaveMatchData(GameCode);
                await _gameService.SaveStatistics(matchData);
            }
            else if (Type == GameType.SinglePlayer)
            {
                await _gameService.NotifyGroupAsync(GameCode, "GameStateChanged", new FinalSingleGameData(result, State, Board.Cells));
            }

            await _gameService.RemoveGame(GameCode);

            Debug();
        }

        private void Debug()
        {

            Console.WriteLine($"State: {State}");
            Board.Debug();
            if (Type == GameType.MultiPlayer)
            {
                Console.WriteLine("Player1TimeRemaining: " + Math.Round(Player1TimeRemaining.TotalSeconds) + "s");
                Console.WriteLine("Player2TimeRemaining: " + Math.Round(Player2TimeRemaining.TotalSeconds) + "s");
            }
            Console.WriteLine();
        }

        public void Dispose() => TimeoutTimer?.Dispose();
    }
}
