using Backend.Data;
using Backend.GameLogic.Entities;
using Backend.GameLogic.Logic;
using Backend.Hubs;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Backend.Services
{
    public class GameService(IHubContext<GameHub> hubContext, IServiceScopeFactory scopeFactory)
    {
        private readonly IHubContext<GameHub> _hubContext = hubContext;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

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

            return new InitialGameData(ownPlayerId, otherPlayerName, game.State, game.Board.Cells, [(int)game.Player1TimeRemaining.TotalSeconds, (int)game.Player2TimeRemaining.TotalSeconds]);
        }

        public bool TryJoinRoom(string gameCode, Player player)
        {
            return Games.TryGetValue(gameCode, out var game) && game.TryJoin(player);
        }

        public async Task NotifyGroupAsync(string gameCode, string eventName, object data)
        {
            await _hubContext.Clients.Group(gameCode).SendAsync(eventName, data);
        }

        public async Task SaveMatchData(string gameCode)
        {
            try
            {
                if (!Games.TryGetValue(gameCode, out var game) || game.PlayerCount != 2)
                {
                    return;
                }

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var matchData = new Match()
                {
                    Id = Guid.NewGuid(),
                    PlayerOneUserId = game.Players[0]?.UserId ?? -1,
                    PlayerTwoUserId = game.Players[1]?.UserId ?? -1,
                    WinnerUserId = game.Winner?.UserId,
                    Steps = game.Board.Steps
                };

                dbContext.Matches.Add(matchData);
                Console.WriteLine("Saving match data to the database...");
                await dbContext.SaveChangesAsync();
                Console.WriteLine("Match data saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving match data: {ex.Message}");
            }
        }


        private async Task RemoveAllConnectionsFromGroup(string gameCode)
        {
            if (!Games.TryGetValue(gameCode, out var game)) return;

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
