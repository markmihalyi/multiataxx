using AI.Abstractions;
using Backend.DTOs;
using Backend.GameBase.Entities;
using Backend.GameBase.Logic;
using Backend.Hubs;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Backend.Services
{
    public class GameService(IHubContext<GameHub> hubContext, ScopedExecutor scopedExecutor, IServiceProvider provider)
    {
        private readonly IHubContext<GameHub> _hubContext = hubContext;
        private readonly ScopedExecutor _scopedExecutor = scopedExecutor;
        private readonly IServiceProvider _provider = provider;

        private readonly ConcurrentDictionary<string, Game> Games = [];

        public string? CreateMultiPlayerGame(int userId, BoardSize boardSize, double turnMinutes)
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

            var newGame = new Game(_provider, gameCode, GameType.MultiPlayer, boardSize, turnMinutes);
            Games.TryAdd(gameCode, newGame);

            return gameCode;
        }

        public string CreateSinglePlayerGame(BoardSize boardSize, GameDifficulty difficulty)
        {
            string gameCode = GenerateGameCode();
            while (Games.ContainsKey(gameCode))
            {
                gameCode = GenerateGameCode();
            }

            var newGame = new Game(_provider, gameCode, GameType.SinglePlayer, boardSize, difficulty);
            Games.TryAdd(gameCode, newGame);

            return gameCode;
        }

        public async Task<List<MatchHistoryData>> GetMatchHistory(int userId)
        {
            var user = await _scopedExecutor.RunInScope(async dbContext => await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId));
            if (user == null)
            {
                return [];
            }

            var matches = await _scopedExecutor.RunInScope(async dbContext =>
                await dbContext.Matches
                .Where(m => m.PlayerOneUserId == userId || m.PlayerTwoUserId == userId)
                .Select(m => new MatchHistoryData(m.Id, m.WinnerUserId == userId ? MatchResult.Won : m.WinnerUserId == null ? MatchResult.Draw : MatchResult.Lost, m.Duration, m.Date))
                .ToListAsync()
            );
            return matches;
        }

        public async Task<MatchDetails?> GetMatchDetails(int userId, Guid matchGuid)
        {
            var matchData = await _scopedExecutor.RunInScope(async dbContext =>
                await dbContext.Matches
                .FirstOrDefaultAsync(m => m.Id == matchGuid && (m.PlayerOneUserId == userId || m.PlayerTwoUserId == userId))
            );
            if (matchData == null)
            {
                return null;
            }

            var playerOne = await _scopedExecutor.RunInScope(async dbContext => await dbContext.Users.FirstOrDefaultAsync(u => u.Id == matchData.PlayerOneUserId));
            var playerTwo = await _scopedExecutor.RunInScope(async dbContext => await dbContext.Users.FirstOrDefaultAsync(u => u.Id == matchData.PlayerTwoUserId));
            if (playerOne == null || playerTwo == null)
            {
                return null;
            }

            var matchResult = matchData.WinnerUserId == userId ? MatchResult.Won : matchData.WinnerUserId == null ? MatchResult.Draw : MatchResult.Lost;
            return new MatchDetails(matchData.Id, matchResult, [playerOne.Username, playerTwo.Username], matchData.Steps);
        }

        public async Task<UserStatistics> GetUserStatistics(int userId)
        {
            var statistics = await _scopedExecutor.RunInScope(async dbContext =>
                await dbContext.UserStatistics.Where(s => s.UserId == userId).FirstOrDefaultAsync()
            );
            if (statistics == null)
            {
                return new UserStatistics(userId);
            }
            return statistics;
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

        public Game? GetGameOfClient(string connectionId)
        {
            return Games.Values.FirstOrDefault(g => g.Players.Any(p => p?.ConnectionId == connectionId));
        }

        public object? GetInitialGameData(string gameCode, int? userId)
        {
            if (!Games.TryGetValue(gameCode, out var game))
            {
                return null;
            }

            int ownPlayerId = -1;
            string? otherPlayerName = null;

            if (userId == null)
            {
                ownPlayerId = 0;
                otherPlayerName = "Bot";
            }
            else
            {
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
            }

            if (game.Type == GameType.MultiPlayer)
            {
                return new InitialMultiGameData(game.Type, ownPlayerId, otherPlayerName, game.State, game.Board.Cells, game.RemainingTimes);
            }

            return new InitialSingleGameData(game.Type, ownPlayerId, otherPlayerName, game.State, game.Board.Cells);
        }

        public async Task<bool> TryJoinRoom(string gameCode, Player player)
        {
            Games.TryGetValue(gameCode, out var game);
            if (game == null || (game.Type == GameType.MultiPlayer && player.UserId == null))
            {
                return false;
            }
            return await game.TryJoin(player);
        }

        public async Task NotifyGroupAsync(string gameCode, string eventName, object data)
        {
            await _hubContext.Clients.Group(gameCode).SendAsync(eventName, data);
        }

        public async Task<Match?> SaveMatchData(string gameCode)
        {
            if (!Games.TryGetValue(gameCode, out var game) || game.PlayerCount != 2)
            {
                return null;
            }

            var matchData = new Match()
            {
                Id = Guid.NewGuid(),
                PlayerOneUserId = game.Players[0]?.UserId ?? -1,
                PlayerTwoUserId = game.Players[1]?.UserId ?? -1,
                WinnerUserId = game.Winner?.UserId,
                Steps = game.Board.Steps,
                Date = game.MatchEndTimeUtc,
                Duration = (int)(game.MatchEndTimeUtc - game.MatchStartTimeUtc).TotalSeconds
            };

            Console.WriteLine("Saving match data to the database...");
            await _scopedExecutor.RunInScope(async dbContext =>
            {
                dbContext.Matches.Add(matchData);
                await dbContext.SaveChangesAsync();
            });
            Console.WriteLine("Match data saved successfully.");

            return matchData;
        }

        public async Task SaveStatistics(Match? matchData)
        {
            if (matchData == null) return;

            Console.WriteLine("Saving statistics to the database...");
            await _scopedExecutor.RunInScope(async dbContext =>
            {
                int? player1UserId = matchData.PlayerOneUserId;
                int? player2UserId = matchData.PlayerTwoUserId;
                if (player1UserId == -1 || player2UserId == -1) return;

                var userStatistics1 = await dbContext.UserStatistics.Where(s => s.UserId == player1UserId).FirstOrDefaultAsync();
                if (userStatistics1 == null)
                {
                    userStatistics1 = new UserStatistics((int)player1UserId);
                    dbContext.UserStatistics.Add(userStatistics1);
                }

                var userStatistics2 = await dbContext.UserStatistics.Where(s => s.UserId == player2UserId).FirstOrDefaultAsync();
                if (userStatistics2 == null)
                {
                    userStatistics2 = new UserStatistics((int)player2UserId);
                    dbContext.UserStatistics.Add(userStatistics2);
                }

                if (matchData.WinnerUserId == player1UserId)
                {
                    userStatistics1.Wins++;
                    userStatistics2.Losses++;

                    if (userStatistics1.FastestWinTime == null || userStatistics1.FastestWinTime > matchData.Duration)
                    {
                        userStatistics1.FastestWinTime = matchData.Duration;
                    }
                }
                else if (matchData.WinnerUserId == player2UserId)
                {
                    userStatistics2.Wins++;
                    userStatistics1.Losses++;

                    if (userStatistics2.FastestWinTime == null || userStatistics2.FastestWinTime > matchData.Duration)
                    {
                        userStatistics2.FastestWinTime = matchData.Duration;
                    }
                }
                else
                {
                    userStatistics1.Draws++;
                    userStatistics2.Draws++;
                }

                userStatistics1.TotalTimePlayed += matchData.Duration;
                userStatistics2.TotalTimePlayed += matchData.Duration;
                userStatistics1.AverageGameDuration = userStatistics1.TotalTimePlayed / (userStatistics1.Wins + userStatistics1.Losses + userStatistics1.Draws);
                userStatistics2.AverageGameDuration = userStatistics2.TotalTimePlayed / (userStatistics1.Wins + userStatistics1.Losses + userStatistics1.Draws);

                await dbContext.SaveChangesAsync();
            });
            Console.WriteLine("Statistics saved successfully.");
        }

        private async Task RemoveAllConnectionsFromGroup(string gameCode)
        {
            if (!Games.TryGetValue(gameCode, out var game)) return;

            foreach (var player in game.Players)
            {
                if (player == null || player.ConnectionId == null) continue;
                await _hubContext.Groups.RemoveFromGroupAsync(player.ConnectionId, gameCode);
            }
        }

        private static string GenerateGameCode()
        {
            return Guid.NewGuid().ToString()[..8];
        }
    }
}
