using tictactoe.api.Models;

namespace tictactoe.api.Helpers
{
    public static class GameRules
    {
        private static readonly int[][] WinningCombinations =
        [
            //rows
            [0, 1, 2],
            [3, 4, 5],
            [6, 7, 8],
            //columns
            [0, 3, 6],
            [1, 4, 7],
            [2, 5, 8],
            //diagonals
            [0, 4, 8],
            [2, 4, 6]
        ];
        private static readonly int[] Corners =
        [
            0, 2, 6, 8
        ];

        // --------------------------------------------------
        // 1. Validate row and column
        // --------------------------------------------------

        public static bool IsValidPosition(int row, int column)
        {
            return row >= 0 &&
                   row < 3 &&
                   column >= 0 &&
                   column < 3;
        }

        // --------------------------------------------------
        // 2. Convert row/column to cell index
        // --------------------------------------------------

        public static int GetCellIndex(int row, int column)
        {
            if (!IsValidPosition(row, column))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(row),
                    "Row and column must be between 0 and 2."
                );
            }

            return row * 3 + column;
        }

        // --------------------------------------------------
        // 3. Check whether a cell is empty
        // --------------------------------------------------

        public static bool IsCellEmpty(
            string?[] board,
            int index)
        {
            return index >= 0 &&
                   index < board.Length &&
                   board[index] is null;
        }

        // --------------------------------------------------
        // 4. Detect winning cells
        // --------------------------------------------------

        public static int[]? FindWinningCells(string?[] board)
        {
            foreach (var combination in WinningCombinations)
            {
                var first = board[combination[0]];

                if (first is not null &&
                    first == board[combination[1]] &&
                    first == board[combination[2]])
                {
                    return combination;
                }
            }

            return null;
        }
        // --------------------------------------------------
        // 5. Detect winner
        // --------------------------------------------------

        public static Player? GetWinner(string?[] board)
        {
            var winningCells = FindWinningCells(board);

            if (winningCells is null)
            {
                return null;
            }

            var symbol = board[winningCells[0]];

            return Enum.Parse<Player>(symbol!);
        }

        // --------------------------------------------------
        // 6. Detect draw
        // --------------------------------------------------

        public static bool IsDraw(string?[] board)
        {
            return FindWinningCells(board) is null &&
                   board.All(cell => cell is not null);
        }

        // --------------------------------------------------
        // 7. Check whether the board is completed
        // --------------------------------------------------

        public static bool IsGameCompleted(string?[] board)
        {
            return FindWinningCells(board) is not null ||
                   board.All(cell => cell is not null);
        }

        // --------------------------------------------------
        // 8. Check whether a player can win in one move
        // --------------------------------------------------

        public static int? FindImmediateWinningMove(
            string?[] board,
            Player player)
        {
            var symbol = player.ToString();

            for (var index = 0; index < board.Length; index++)
            {
                if (board[index] is not null)
                {
                    continue;
                }

                // Temporarily place the player's symbol.
                board[index] = symbol;

                var winningCells = FindWinningCells(board);

                // Always restore the original board.
                board[index] = null;

                if (winningCells is not null)
                {
                    return index;
                }
            }

            return null;
        }

        // --------------------------------------------------
        // 9. Select computer move
        //
        // Priority:
        // 1. O winning move
        // 2. Block X winning move
        // 3. Center
        // 4. Corner
        // 5. Any empty cell
        // --------------------------------------------------

        public static int ChooseComputerMove(string?[] board)
        {
            // Priority 1: Win immediately if possible.
            var winningMove =
                FindImmediateWinningMove(board, Player.O);

            if (winningMove.HasValue)
            {
                return winningMove.Value;
            }

            // Priority 2: Block the human's winning move.
            var blockingMove =
                FindImmediateWinningMove(board, Player.X);

            if (blockingMove.HasValue)
            {
                return blockingMove.Value;
            }

            // Priority 3: Take the center.
            if (board[4] is null)
            {
                return 4;
            }

            // Priority 4: Take an available corner.
            foreach (var corner in Corners)
            {
                if (board[corner] is null)
                {
                    return corner;
                }
            }

            // Priority 5: Take any available cell.
            for (var index = 0; index < board.Length; index++)
            {
                if (board[index] is null)
                {
                    return index;
                }
            }

            // No available cells.
            return -1;
        }
    }
}
