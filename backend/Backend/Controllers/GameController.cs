using AI.Abstractions;
using Backend.DTOs;
using Backend.GameBase.Entities;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/game")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class GameController(GameService gameService) : ControllerBase
    {
        private readonly GameService _gameService = gameService;

        /// <summary>
        /// Create new single/multiplayer game
        /// </summary>
        /// <param name="body">Data required to create a game</param>
        /// <response code="200">If the game is created successfully</response>
        /// <response code="400">If the request body is invalid</response>
        /// <response code="401">If user is not logged in when creating a multiplayer game</response>
        /// <response code="409">If a multiplayer game is already in progress for the current user</response>
        [HttpPost]
        [ProducesResponseType(typeof(CreateGameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult CreateGame([FromBody] CreateGameRequestBody body)
        {
            if (!Enum.TryParse<GameType>(body.GameType, true, out var gameType))
            {
                return BadRequest(new ErrorResponse("Game type is invalid."));
            }

            if (!Enum.TryParse<BoardSize>(body.BoardSize, true, out var boardSize))
            {
                return BadRequest(new ErrorResponse("Board size is invalid."));
            }

            if (gameType == GameType.MultiPlayer)
            {
                var userIdentifier = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdentifier == null)
                {
                    return Unauthorized(new ErrorResponse("Login is required for multiplayer mode."));
                }

                if (body.TurnMinutes == null || body.TurnMinutes < 0.5 || body.TurnMinutes > 30)
                {
                    return BadRequest(new ErrorResponse("Turn minutes parameter is invalid."));
                }

                int userId = Convert.ToInt32(userIdentifier?.Value);
                var gameCode = _gameService.CreateMultiPlayerGame(userId, boardSize, (double)body.TurnMinutes);
                if (gameCode == null)
                {
                    return Conflict(new ErrorResponse("You already have a game in progress."));
                }
                return Ok(new CreateGameResponse("You have created a multiplayer game.", gameCode));
            }
            else if (gameType == GameType.SinglePlayer)
            {
                if (!Enum.TryParse<GameDifficulty>(body.Difficulty, true, out var difficulty))
                {
                    return BadRequest(new ErrorResponse("Game type is invalid."));
                }

                var gameCode = _gameService.CreateSinglePlayerGame(boardSize, difficulty);
                return Ok(new CreateGameResponse("You have created a single player game.", gameCode));
            }
            else
            {
                return BadRequest(new ErrorResponse("Game type is invalid."));
            }
        }

        /// <summary>
        /// Get match history
        /// </summary>
        /// <response code="200">If the game history is successfully retrieved</response>
        [HttpGet("history")]
        [Authorize]
        [ProducesResponseType(typeof(List<MatchHistoryData>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMatchHistory()
        {
            int userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var matchHistory = await _gameService.GetMatchHistory(userId);
            return Ok(matchHistory);
        }

        /// <summary>
        /// Get details of a specific match
        /// </summary>
        /// <param name="matchId">The ID of the match to retrieve details for</param>
        /// <response code="200">If the match details are successfully retrieved</response>
        /// <response code="400">If an invalid match ID is provided</response>
        /// <response code="404">If no match with the given ID is found</response>
        [HttpGet("history/{matchId}")]
        [Authorize]
        [ProducesResponseType(typeof(MatchDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMatchDetails(string matchId)
        {
            if (!Guid.TryParse(matchId, out var matchGuid))
            {
                return BadRequest(new ErrorResponse("Invalid match ID provided."));
            }

            int userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var matchDetails = await _gameService.GetMatchDetails(userId, matchGuid);
            if (matchDetails == null)
            {
                return NotFound(new ErrorResponse("Match not found."));
            }
            return Ok(matchDetails);
        }
    }
}
