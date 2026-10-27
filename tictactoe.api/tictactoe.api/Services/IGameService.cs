using tictactoe.api.Models;

namespace tictactoe.api.Services
{
    public interface IGameService
    {
        GameStateResponseDTO CreateGame(GameMode mode);

        GameStateResponseDTO GetGame(Guid gameId);

        GameStateResponseDTO MakeMove(
            Guid gameId,
            MoveRequest request);

        GameStateResponseDTO UndoMove(Guid gameId);

        GameStateResponseDTO ResetGame(Guid gameId);

        Scoreboard GetScoreboard();

        Scoreboard ResetScoreboard();
    }
}
