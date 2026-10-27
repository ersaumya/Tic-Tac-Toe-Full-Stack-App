namespace tictactoe.api.Models
{
    public class GameState
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string[] Board { get; set; } =new string[9];

        public Player CurrentPlayer { get; set; } =Player.X;

        public GameMode Mode { get; set; } =GameMode.TwoPlayer;

        public GameStatus Status { get; set; } =GameStatus.InProgress;

        public Player? Winner { get; set; }

        public List<int> WinningCells { get; set; } = [];

        public List<Move> MoveHistory { get; set; } = [];

        public bool ScoreUpdated { get; set; }

        public DateTime CreatedAtUtc { get; set; } =DateTime.UtcNow;
        // Internal synchronization object.
        // It must not be serialized into the API response.
        internal object SyncRoot { get; } = new();
    }
}
