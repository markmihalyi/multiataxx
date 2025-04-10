using Backend.GameLogic.Entities;
using Backend.GameLogic.Logic;
using Backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Backend.Services
{
    public class GameService(IHubContext<GameHub> hubContext)
    {
        private readonly IHubContext<GameHub> _hubContext = hubContext;

        private readonly ConcurrentDictionary<string, Game> Games = [];

        public string? CreateGame(int userId, BoardSize boardSize, double turnMinutes)
        {
            var game = GetGameOfUser(userId);
            if (game != null)
            {
                return null;
            }

            string gameCode = GenerateGameCode();
            while (Games.ContainsKey(gameCode))
            {
                gameCode = GenerateGameCode();
            }

            var newGame = new Game(this, gameCode, boardSize, turnMinutes);
            Games.TryAdd(gameCode, newGame);

            return gameCode;
        }

        public async Task<bool> RemoveGame(string gameCode)
        {
            await RemoveAllConnectionsFromGroup(gameCode);
            if (Games.TryRemove(gameCode, out var game))
            {
                game.Dispose();
                return true;
            }
            return false;
        }

        public Game? GetGameOfUser(int userId)
        {
            return Games.Values.FirstOrDefault(g => g.Players.Any(p => p?.UserId == userId));
        }

        public InitialGameData? GetInitialGameData(string gameCode, int userId)
        {
            if (!Games.TryGetValue(gameCode, out var game))
            {
                return null;
            }

            int ownPlayerId = -1;
            string? otherPlayerName = null;

            foreach (var (player, index) in game.Players.Select((player, index) => (player, index)))
            {
                if (player?.UserId == userId)
                {
                    ownPlayerId = index;
                }
                else
                {
                    otherPlayerName = player?.Name;
                }
            }

            return new InitialGameData(ownPlayerId, otherPlayerName, game.State, game.Board.Cells);
        }

        public bool TryJoinRoom(string gameCode, Player player)
        {
            return Games.TryGetValue(gameCode, out var game) && game.TryJoin(player);
        }

        public async Task NotifyGroupAsync(string gameCode, string eventName, object data)
        {
            await _hubContext.Clients.Group(gameCode).SendAsync(eventName, data);
        }

        private async Task RemoveAllConnectionsFromGroup(string gameCode)
        {
            var game = Games[gameCode];
            if (game == null) return;

            foreach (var player in game.Players)
            {
                if (player == null) continue;
                await _hubContext.Groups.RemoveFromGroupAsync(player.ConnectionId, gameCode);
            }
        }

        private static string GenerateGameCode()
        {
            return Guid.NewGuid().ToString()[..8];
        }
    }
}
