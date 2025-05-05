using Backend.GameBase.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Match
    {
        [Key]
        public required Guid Id { get; set; }

        [Required]
        public int PlayerOneUserId { get; set; }

        [Required]
        public int PlayerTwoUserId { get; set; }

        [Required]
        public int? WinnerUserId { get; set; }

        [Required]
        public required List<CellState[,]> Steps { get; set; }

        [Required]
        public required DateTime Date { get; set; }

        [Required]
        public required int Duration { get; set; }
    }
}
