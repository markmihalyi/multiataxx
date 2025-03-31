using Backend.GameLogic.Entities;
using Backend.GameLogic.Logic;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs
{
    [Authorize]
    public class GameHub(GameService gameService) : Hub
    {
        private readonly GameService _gameService = gameService;

        public async Task JoinGame(string gameCode)
        {
            int userId = Convert.ToInt32(Context?.UserIdentifier);
            if (Context == null || userId == 0) return;

            string playerName = Context.User?.Identity?.Name ?? "Player";
            var player = new Player(userId, Context.ConnectionId, playerName);
            if (!_gameService.TryJoinRoom(gameCode, player))
            {
                await Clients.Caller.SendAsync("JoinFailed");
                return;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, gameCode);

            var gameData = _gameService.GetInitialGameData(gameCode, userId);
            await Clients.Caller.SendAsync("JoinSuccessful", gameData);
            await Clients.OthersInGroup(gameCode).SendAsync("PlayerJoined", playerName);
        }

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
            int userId = Convert.ToInt32(Context?.UserIdentifier);
            if (Context == null || userId == 0) return;


            var game = _gameService.GetGameOfUser(userId);
            if (game == null) return;

            string status = await game.AttemptMove(userId, new Point(startRow, startColumn), new Point(destRow, destColumn));
            if (status == "MoveCompleted") return;

            await Clients.Caller.SendAsync(status);
        }
    }
}
