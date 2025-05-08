using AI.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Booster
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public BoosterActionType Action { get; set; }

        // navigation properties
        public List<OwnedBooster> OwnedByUsers { get; set; } = [];
    }

    public enum BoosterActionType
    {
        Tip = GameDifficulty.Easy,
        SmartTip = GameDifficulty.Medium,
        ProTip = GameDifficulty.Hard
    }
}
