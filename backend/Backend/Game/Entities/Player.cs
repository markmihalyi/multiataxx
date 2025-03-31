namespace Backend.GameLogic.Entities
{
    public class Player(int userId, string connectionId, string name)
    {
        public int UserId { get; } = userId;

        public string ConnectionId { get; } = connectionId;

        public string Name { get; } = name;

        public bool IsReady { get; set; } = false;

        public override bool Equals(object? obj) => obj is Player other && UserId == other.UserId;

        public override int GetHashCode() => UserId.GetHashCode();
    }
}
