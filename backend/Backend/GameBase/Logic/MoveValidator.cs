using AI.Abstractions;

namespace Backend.GameBase.Logic
{
    public record Point(int X, int Y)
    {
        public int DistanceTo(Point other)
        {
            return Math.Max(Math.Abs(other.X - X), Math.Abs(other.Y - Y));
        }
    }

    public enum MoveType
    {
        SIMPLE_MOVE,
        JUMP,
        INVALID
    }

    public class MoveValidator
    {
        public static MoveType ValidateMove(Point start, Point destination, CellState[,] cells, CellState ownCellState)
        {
            MoveType moveType = MoveType.INVALID;

            // Check starting and destination cells are within the range of the game board
            if (!IsWithinBounds(start, cells) || !IsWithinBounds(destination, cells))
            {
                return moveType;
            }

            // Check the state of the initial cell
            if (cells[start.X, start.Y] != ownCellState)
            {
                return moveType;
            }

            // Check the state of the target cell
            if (cells[destination.X, destination.Y] != CellState.Empty)
            {
                return moveType;
            }

            // Check the type of movement
            if (start.DistanceTo(destination) == 1)
            {
                moveType = MoveType.SIMPLE_MOVE;
            }
            else if (start.DistanceTo(destination) == 2)
            {
                moveType = MoveType.JUMP;
            }

            return moveType;
        }

        private static bool IsWithinBounds(Point point, CellState[,] cells)
        {
            return point.X >= 0 && point.X < cells.GetLength(0) && point.Y >= 0 && point.Y < cells.GetLength(1);
        }
    }
}
