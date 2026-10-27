using System.Collections.Concurrent;
using tictactoe.api.Models;

namespace tictactoe.api.Repositories
{
    public sealed class InMemoryGameRepository:IGameRepository
    {
        private readonly ConcurrentDictionary<Guid, GameState> _games =
        new();

        private readonly Scoreboard _scoreboard = new();

        private readonly object _scoreboardLock = new();

        public GameState Create(GameMode mode)
        {
            var game = new GameState
            {
                Id = Guid.NewGuid(),
                Board = new string?[9],
                CurrentPlayer = Player.X,
                Mode = mode,
                Status = GameStatus.InProgress,
                Winner = null,
                WinningCells = [],
                MoveHistory = [],
                ScoreUpdated = false
            };

            _games[game.Id] = game;

            return game;
        }

        public GameState? GetById(Guid id)
        {
            _games.TryGetValue(id, out var game);

            return game;
        }

        public Scoreboard GetScoreboard()
        {
            lock (_scoreboardLock)
            {
                return new Scoreboard
                {
                    XWins = _scoreboard.XWins,
                    OWins = _scoreboard.OWins,
                    Draws = _scoreboard.Draws
                };
            }
        }

        public void ResetScoreboard()
        {
            lock (_scoreboardLock)
            {
                _scoreboard.XWins = 0;
                _scoreboard.OWins = 0;
                _scoreboard.Draws = 0;
            }
        }

        public void RecordResult(GameState game)
        {
            lock (_scoreboardLock)
            {
                if (game.ScoreUpdated)
                {
                    return;
                }

                switch (game.Status)
                {
                    case GameStatus.Won when game.Winner == Player.X:
                        _scoreboard.XWins++;
                        break;

                    case GameStatus.Won when game.Winner == Player.O:
                        _scoreboard.OWins++;
                        break;

                    case GameStatus.Draw:
                        _scoreboard.Draws++;
                        break;

                    default:
                        return;
                }

                game.ScoreUpdated = true;
            }
        }
    }
}
