namespace Backend.Models
{
    public class OwnedBooster
    {
        public int UserId { get; set; }
        public int BoosterId { get; set; }
        public int Amount { get; set; }

        // navigation properties
        public User User { get; set; } = null!;
        public Booster Booster { get; set; } = null!;
    }
}
