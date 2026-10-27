namespace tictactoe.api.Models
{
    public record GameStateResponseDTO(
        Guid GameId,
        string?[] Board,
        Player CurrentPlayer,
        GameMode Mode,
        GameStatus Status,
        Player? Winner,
        List<int> WinningCells,
        List<Move> MoveHistory,
        bool CanUndo
    );
   
}
