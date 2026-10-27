namespace tictactoe.api.Exceptions
{
    public sealed class GameRuleException:Exception
    {
        public int StatusCode { get; }

        public GameRuleException(
            string message,
            int statusCode = 400)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
