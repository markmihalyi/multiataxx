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
        /// Create new game
        /// </summary>
        /// <param name="body">Data required to create a game</param>
        /// <response code="200">If the game is created successfully</response>
        /// <response code="409">If the user already has a game in progress</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(CreateGameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult CreateGame([FromBody] CreateGameRequestBody body)
        {
            if (!Enum.TryParse<BoardSize>(body.BoardSize, true, out var boardSize))
            {
                return BadRequest(new ErrorResponse("Board size is invalid."));
            }

            if (body.TurnMinutes < 0.5 || body.TurnMinutes > 30)
            {
                return BadRequest(new ErrorResponse("Turn minutes parameter is invalid."));
            }

            int userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var gameCode = _gameService.CreateGame(userId, boardSize, body.TurnMinutes);
            if (gameCode == null)
            {
                return Conflict(new ErrorResponse("You already have a game in progress."));
            }

            return Ok(new CreateGameResponse("You have created a new game.", gameCode));

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
        /// <response code="404">If no match with the given ID is found</response>
        [HttpGet("history/{matchId}")]
        [Authorize]
        [ProducesResponseType(typeof(MatchDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMatchDetails(string matchId)
        {
            int userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var matchDetails = await _gameService.GetMatchDetails(userId, Guid.Parse(matchId));
            if (matchDetails == null)
            {
                return NotFound(new ErrorResponse("Match not found."));
            }
            return Ok(matchDetails);
        }
    }
}
