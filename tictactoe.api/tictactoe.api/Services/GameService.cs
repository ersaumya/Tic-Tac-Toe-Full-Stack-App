using tictactoe.api.Exceptions;
using tictactoe.api.Helpers;
using tictactoe.api.Models;
using tictactoe.api.Repositories;

namespace tictactoe.api.Services
{
    public sealed class GameService:IGameService
    {
        private readonly IGameRepository _repository;

        public GameService(IGameRepository repository)
        {
            _repository = repository;
        }

        // --------------------------------------------------
        // 1. Create game
        // --------------------------------------------------

        public GameStateResponseDTO CreateGame(GameMode mode)
        {
            var game = _repository.Create(mode);

            return ToResponse(game);
        }

        // --------------------------------------------------
        // 2. Get game
        // --------------------------------------------------

        public GameStateResponseDTO GetGame(Guid gameId)
        {
            var game = GetRequiredGame(gameId);

            lock (game.SyncRoot)
            {
                return ToResponse(game);
            }
        }

        // --------------------------------------------------
        // 3. Make player move
        // --------------------------------------------------

        public GameStateResponseDTO MakeMove(
            Guid gameId,
            MoveRequest request)
        {
            var game = GetRequiredGame(gameId);

            lock (game.SyncRoot)
            {
                ValidateMove(game, request);

                ApplyMove(
                    game,
                    request.Player,
                    request.Row,
                    request.Column);

                // If X has not won or drawn, the computer can play.
                if (game.Mode == GameMode.Computer &&
                    game.Status == GameStatus.InProgress)
                {
                    ExecuteComputerMove(game);
                }

                return ToResponse(game);
            }
        }

        // --------------------------------------------------
        // 4. Validate move
        // --------------------------------------------------

        private static void ValidateMove(
            GameState game,
            MoveRequest request)
        {
            if (game.Status != GameStatus.InProgress)
            {
                throw new GameRuleException(
                    "The game has already completed.",
                    409);
            }

            if (!GameRules.IsValidPosition(
                    request.Row,
                    request.Column))
            {
                throw new GameRuleException(
                    "Row and column must be between 0 and 2.",
                    400);
            }

            if (request.Player != game.CurrentPlayer)
            {
                throw new GameRuleException(
                    $"It is {game.CurrentPlayer}'s turn.",
                    409);
            }

            if (game.Mode == GameMode.Computer &&
                request.Player != Player.X)
            {
                throw new GameRuleException(
                    "Only player X can submit human moves in Computer Mode.",
                    409);
            }

            var index = GameRules.GetCellIndex(
                request.Row,
                request.Column);

            if (!GameRules.IsCellEmpty(
                    game.Board,
                    index))
            {
                throw new GameRuleException(
                    "The selected cell is already occupied.",
                    409);
            }
        }

        // --------------------------------------------------
        // 5. Apply a move
        // --------------------------------------------------

        private void ApplyMove(
            GameState game,
            Player player,
            int row,
            int column)
        {
            var index = GameRules.GetCellIndex(
                row,
                column);

            game.Board[index] = player.ToString();

            game.MoveHistory.Add(
                new Move(
                    MoveNumber: game.MoveHistory.Count + 1,
                    Player: player,
                    Row: row,
                    Column: column
                ));

            UpdateGameStatus(game, player);
        }

        // --------------------------------------------------
        // 6. Update win/draw status and current player
        // --------------------------------------------------

        private void UpdateGameStatus(
            GameState game,
            Player lastPlayer)
        {
            var winningCells =
                GameRules.FindWinningCells(game.Board);

            if (winningCells is not null)
            {
                game.Status = GameStatus.Won;
                game.Winner = lastPlayer;

                game.WinningCells = winningCells.ToList();

                _repository.RecordResult(game);

                return;
            }

            if (GameRules.IsDraw(game.Board))
            {
                game.Status = GameStatus.Draw;
                game.Winner = null;
                game.WinningCells = [];

                _repository.RecordResult(game);

                return;
            }

            game.Status = GameStatus.InProgress;
            game.Winner = null;
            game.WinningCells = [];

            game.CurrentPlayer =
                lastPlayer == Player.X
                    ? Player.O
                    : Player.X;
        }

        // --------------------------------------------------
        // 7. Computer move
        // --------------------------------------------------

        private void ExecuteComputerMove(GameState game)
        {
            if (game.Status != GameStatus.InProgress)
            {
                return;
            }

            if (game.CurrentPlayer != Player.O)
            {
                return;
            }

            var index =
                GameRules.ChooseComputerMove(game.Board);

            if (index < 0)
            {
                return;
            }

            var row = index / 3;
            var column = index % 3;

            ApplyMove(
                game,
                Player.O,
                row,
                column);
        }

        // --------------------------------------------------
        // 8. Undo
        // --------------------------------------------------

        public GameStateResponseDTO UndoMove(Guid gameId)
        {
            var game = GetRequiredGame(gameId);

            lock (game.SyncRoot)
            {
                if (game.Status != GameStatus.InProgress)
                {
                    throw new GameRuleException(
                        "Undo is disabled after the game has completed.",
                        409);
                }

                if (game.MoveHistory.Count == 0)
                {
                    throw new GameRuleException(
                        "There are no moves to undo.",
                        409);
                }

                var movesToRemove =
                    game.Mode == GameMode.TwoPlayer
                        ? 1
                        : Math.Min(2, game.MoveHistory.Count);

                for (var i = 0; i < movesToRemove; i++)
                {
                    var lastIndex = game.MoveHistory.Count - 1;

                    var lastMove = game.MoveHistory[lastIndex];

                    var cellIndex = lastMove.CellIndex;

                    game.Board[cellIndex] = null;

                    game.MoveHistory.RemoveAt(lastIndex);
                }

                // Rebuild move numbers after undo.
                for (var i = 0; i < game.MoveHistory.Count; i++)
                {
                    var move = game.MoveHistory[i];

                    game.MoveHistory[i] = move with
                    {
                        MoveNumber = i + 1
                    };
                }

                game.Status = GameStatus.InProgress;
                game.Winner = null;
                game.WinningCells = [];

                game.CurrentPlayer =
                    game.MoveHistory.Count % 2 == 0
                        ? Player.X
                        : Player.O;

                return ToResponse(game);
            }
        }

        // --------------------------------------------------
        // 9. Reset game
        // --------------------------------------------------

        public GameStateResponseDTO ResetGame(Guid gameId)
        {
            var existingGame = GetRequiredGame(gameId);

            GameMode mode;

            lock (existingGame.SyncRoot)
            {
                mode = existingGame.Mode;
            }

            // Create a new game ID.
            // The shared scoreboard remains unchanged.
            return CreateGame(mode);
        }

        // --------------------------------------------------
        // 10. Scoreboard
        // --------------------------------------------------

        public Scoreboard GetScoreboard()
        {
            return _repository.GetScoreboard();
        }

        public Scoreboard ResetScoreboard()
        {
            _repository.ResetScoreboard();

            return _repository.GetScoreboard();
        }

        // --------------------------------------------------
        // 11. Get game or throw
        // --------------------------------------------------

        private GameState GetRequiredGame(Guid gameId)
        {
            return _repository.GetById(gameId)
                ?? throw new GameRuleException(
                    "Game not found.",
                    404);
        }

        // --------------------------------------------------
        // 12. Map internal game state to response DTO
        // --------------------------------------------------

        private static GameStateResponseDTO ToResponse(
            GameState game)
        {
            var canUndo =
                game.Status == GameStatus.InProgress &&
                game.MoveHistory.Count > 0;

            return new GameStateResponseDTO(
                GameId: game.Id,
                Board: game.Board.ToArray(),
                CurrentPlayer: game.CurrentPlayer,
                Mode: game.Mode,
                Status: game.Status,
                Winner: game.Winner,
                WinningCells: game.WinningCells,
                MoveHistory: game.MoveHistory,
                CanUndo: canUndo
            );
        }
    }
}
