using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UserStatistics(int userId)
    {
        [Key]
        public int UserId { get; set; } = userId;

        public int Wins { get; set; } = 0;

        public int Losses { get; set; } = 0;

        public int Draws { get; set; } = 0;

        public int TotalTimePlayed { get; set; } = 0;

        public int AverageGameDuration { get; set; } = 0;

        public int? FastestWinTime { get; set; } = null;
    }
}
