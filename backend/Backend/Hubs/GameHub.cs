using Backend.GameBase.Entities;
using Backend.GameBase.Logic;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs
{
    public class GameHub(GameService gameService) : Hub
    {
        private readonly GameService _gameService = gameService;

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

            var game = _gameService.GetGameOfClient(Context.ConnectionId);
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
    }
}
