using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tictactoe.api.Exceptions;
using tictactoe.api.Models;
using tictactoe.api.Services;

namespace tictactoe.api.Controllers
{
    [Route("api/games")]
    [ApiController]
    [Produces("application/json")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }
        /// <summary>
        /// Create a new game session.
        /// </summary>
        /// <param name="request">
        /// Contains the game mode, such as two-player or computer mode.
        /// </param>
        /// <returns>The initial state of the newly created game.</returns>
        /// <response code="201">Game created successfully.</response>
        /// <response code="400">The request contains invalid game configuration.</response>
        [HttpPost]
        [ProducesResponseType(typeof(GameStateResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<GameStateResponseDTO> CreateGame(
            [FromBody] CreateGameRequestDTO request)
        {
            try
            {
                var game = _gameService.CreateGame(request.Mode);

                return CreatedAtAction(
                    nameof(GetGame),
                    new { id = game.GameId },
                    game);
            }
            catch (GameRuleException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid game request",
                    Detail = ex.Message
                });
            }
        }
        /// <summary>
        /// Get current game state.
        /// </summary>
        /// <param name="id">Unique identifier of the game.</param>
        /// <returns>The current board, player, status, moves, and scoreboard.</returns>
        /// <response code="200">Game retrieved successfully.</response>
        /// <response code="404">The specified game was not found.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(GameStateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult<GameStateResponseDTO> GetGame(Guid id)
        {
            try
            {
                var game = _gameService.GetGame(id);

                return Ok(game);
            }
            catch (GameRuleException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Game not found",
                    Detail = ex.Message
                });
            }
        }
        /// <summary>
        /// Submit a player move.
        /// </summary>
        /// <param name="id">Unique identifier of the game.</param>
        /// <param name="request">
        /// Contains the row, column, and player making the move.
        /// </param>
        /// <returns>The updated game state after the move.</returns>
        /// <response code="200">Move completed successfully.</response>
        /// <response code="400">
        /// The move is invalid, the cell is occupied, or the game is already over.
        /// </response>
        /// <response code="404">The specified game was not found.</response>
        [HttpPost("{id:guid}/moves")]
        [ProducesResponseType(typeof(GameStateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult<GameStateResponseDTO> MakeMove(
            Guid id,
            [FromBody] MoveRequest request)
        {
            try
            {
                var game = _gameService.MakeMove(id, request);

                return Ok(game);
            }
            catch (GameRuleException ex)
            {
                var statusCode = ex.StatusCode;

                return Problem(
                    statusCode: statusCode,
                    title: statusCode == StatusCodes.Status404NotFound
                        ? "Game not found"
                        : "Invalid move",
                    detail: ex.Message);
            }
        }

        /// <summary>
        /// Undo the most recent move or moves according to the game mode.
        /// </summary>
        /// <param name="id">Unique identifier of the game.</param>
        /// <returns>The updated game state after undoing the move.</returns>
        /// <response code="200">Move undone successfully.</response>
        /// <response code="400">The move cannot be undone.</response>
        /// <response code="404">The specified game was not found.</response>
        [HttpPost("{id:guid}/undo")]
        [ProducesResponseType(typeof(GameStateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult<GameStateResponseDTO> UndoMove(Guid id)
        {
            try
            {
                var game = _gameService.UndoMove(id);

                return Ok(game);
            }
            catch (GameRuleException ex)
            {
                return Problem(
                    statusCode: ex.StatusCode,
                    title: "Unable to undo move",
                    detail: ex.Message);
            }
        }

        /// <summary>
        /// Resets a game by creating a fresh game with the same game mode.
        /// </summary>
        /// <param name="id">Unique identifier of the existing game.</param>
        /// <returns>The initial state of the new game.</returns>
        /// <response code="200">Game reset successfully.</response>
        /// <response code="404">The specified game was not found.</response>
        [HttpPost("{id:guid}/reset")]
        [ProducesResponseType(typeof(GameStateResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public ActionResult<GameStateResponseDTO> ResetGame(Guid id)
        {
            try
            {
                var game = _gameService.ResetGame(id);

                return Ok(game);
            }
            catch (GameRuleException ex)
            {
                return NotFound(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Game not found",
                    Detail = ex.Message
                });
            }
        }
    }
}
