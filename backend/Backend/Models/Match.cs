using AI.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Match
    {
        [Key]
        public required Guid Id { get; set; }

        public int PlayerOneUserId { get; set; }

        public int PlayerTwoUserId { get; set; }

        public int? WinnerUserId { get; set; }

        public required List<CellState[,]> Steps { get; set; }

        public required DateTime Date { get; set; }

        public required int Duration { get; set; }
    }
}
