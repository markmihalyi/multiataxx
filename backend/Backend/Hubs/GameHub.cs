using AI.Abstractions;
using Backend.GameBase.Entities;
using Backend.GameBase.Logic;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs
{
    public class GameHub(GameService gameService, IGameAI gameAI) : Hub
    {
        private readonly GameService _gameService = gameService;
        private readonly IGameAI _gameAI = gameAI;

        public async Task JoinGame(string gameCode)
        {
            if (Context == null) return;

            int? userId = Context.UserIdentifier != null ? Convert.ToInt32(Context.UserIdentifier) : null;
            string playerName = Context.User?.Identity?.Name ?? "Player";
            string connectionId = Context.ConnectionId;

            var player = new Player(userId, connectionId, playerName);
            var joinSuccessful = await _gameService.TryJoinRoom(gameCode, player);
            if (!joinSuccessful)
            {
                await Clients.Caller.SendAsync("JoinFailed");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);

            var gameData = _gameService.GetInitialGameData(gameCode, userId);
            await Clients.Caller.SendAsync("JoinSuccessful", gameData);
        }

        [Authorize]
        public async Task PlayerIsReady()
        {
            int userId = Convert.ToInt32(Context?.UserIdentifier);
            if (Context == null || userId == 0) return;

            var game = _gameService.GetGameOfUser(userId);
            if (game == null) return;

            await game.SetPlayerIsReady(userId);
        }

        public async Task AttemptMove(int startRow, int startColumn, int destRow, int destColumn)
        {
            if (Context == null) return;

            int? userId = Context.UserIdentifier != null ? Convert.ToInt32(Context.UserIdentifier) : null;
            var game = userId != null ? _gameService.GetGameOfUser((int)userId) : _gameService.GetGameOfClient(Context.ConnectionId);
            if (game == null) return;

            string status = await game.AttemptMove(userId, new Point(startRow - 1, startColumn - 1), new Point(destRow - 1, destColumn - 1));
            if (status == "MoveCompleted") return;

            await Clients.Caller.SendAsync(status);
        }

        [Authorize]
        public async Task UseBooster(int boosterId)
        {
            int userId = Convert.ToInt32(Context?.UserIdentifier);
            if (Context == null || userId == 0) return;

            var game = _gameService.GetGameOfUser(userId);
            if (game == null) return;

            CellState playerCellState = game.Players[0]?.UserId == userId ? CellState.Player1 : CellState.Player2;
            if ((game.State == GameState.Player1Turn && playerCellState == CellState.Player2) || (game.State == GameState.Player2Turn && playerCellState == CellState.Player1))
            {
                await Clients.Caller.SendAsync("NotYourTurn");
                return;
            }

            var booster = await _gameService.GetBoosterById(boosterId);
            if (booster == null)
            {
                await Clients.Caller.SendAsync("BoosterNotExists");
                return;
            }

            bool canUse = await _gameService.UseBooster(userId, boosterId);
            if (!canUse)
            {
                await Clients.Caller.SendAsync("BoosterNotAvailable");
                return;
            }

            var (startX, startY, destX, destY) = _gameAI.CalculateBotMove(game.Board.Cells, playerCellState, (BoardSize)game.Board.Size, (GameDifficulty)booster.Action);
            await Clients.Caller.SendAsync("TipReceived", startX + 1, startY + 1, destX + 1, destY + 1);
        }
    }
}
