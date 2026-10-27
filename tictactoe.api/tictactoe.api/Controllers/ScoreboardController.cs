using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tictactoe.api.Models;
using tictactoe.api.Services;

namespace tictactoe.api.Controllers
{
    [Route("api/scoreboard")]
    [ApiController]
    public class ScoreboardController : ControllerBase
    {
        private readonly IGameService _gameService;
        public ScoreboardController(IGameService gameService)
        {
            _gameService = gameService;
        }
        /// <summary>
        /// Retrieves the current scoreboard.
        /// </summary>
        /// <returns>The current scores for X, O, and draws.</returns>
        /// <response code="200">Scoreboard retrieved successfully.</response>
        [HttpGet]
        [ProducesResponseType(typeof(Scoreboard), StatusCodes.Status200OK)]
        public ActionResult<Scoreboard> GetScoreboard()
        {
            var scoreboard = _gameService.GetScoreboard();

            return Ok(scoreboard);
        }

        /// <summary>
        /// Resets all scoreboard values to zero.
        /// </summary>
        /// <returns>No content if the scoreboard was reset successfully.</returns>
        /// <response code="204">Scoreboard reset successfully.</response>
        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult ResetScoreboard()
        {
            _gameService.ResetScoreboard();

            return NoContent();
        }

    }
}
