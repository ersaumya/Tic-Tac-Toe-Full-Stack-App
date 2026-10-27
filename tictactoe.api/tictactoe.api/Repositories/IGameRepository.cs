using tictactoe.api.Models;

namespace tictactoe.api.Repositories
{
    public interface IGameRepository
    {
        GameState Create(GameMode mode);

        GameState? GetById(Guid id);

        Scoreboard GetScoreboard();

        void ResetScoreboard();

        void RecordResult(GameState game);
    }
}
