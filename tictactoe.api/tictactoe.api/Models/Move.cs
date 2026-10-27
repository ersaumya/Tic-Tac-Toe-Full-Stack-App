namespace tictactoe.api.Models
{
    public record Move(
        int MoveNumber, 
        Player Player, 
        int Row, 
        int Column
        )
    {
        public int CellIndex => Row * 3 + Column;
    }
   
}
