namespace tictactoe.api.Models
{
    public record MoveRequest(
        Player Player, 
        int Row, 
        int Column
    );
   
}
