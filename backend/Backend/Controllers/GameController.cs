using Backend.DTOs;
using Backend.GameLogic.Entities;
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
        /// Új játék létrehozása
        /// </summary>
        /// <param name="body">Játék létrehozásához szükséges adatok</param>
        /// <response code="200">Sikeres játék létrehozás esetén</response>
        /// <response code="409">Amennyiben a felhasználónak már van egy folyamatban lévő játéka</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(CreateGameResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult CreateGame([FromBody] CreateGameRequestBody body)
        {
            if (!Enum.TryParse<BoardSize>(body.BoardSize, true, out var boardSize))
            {
                return BadRequest(new ErrorResponse("A pályaméret érvénytelen."));
            }

            if (body.TurnMinutes < 0.5 || body.TurnMinutes > 30)
            {
                return BadRequest(new ErrorResponse("A döntési idő érvénytelen."));
            }

            int userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var gameCode = _gameService.CreateGame(userId, boardSize, body.TurnMinutes);
            if (gameCode == null)
            {
                return Conflict(new ErrorResponse("Már van egy folyamatban lévő játékod."));
            }

            return Ok(new CreateGameResponse("Létrehoztál egy új játékot.", gameCode));

        }
    }
}
