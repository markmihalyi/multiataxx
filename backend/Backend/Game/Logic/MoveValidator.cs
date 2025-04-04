﻿using Backend.GameLogic.Entities;

namespace Backend.GameLogic.Logic
{
    public record Point(int X, int Y)
    {
        public int DistanceTo(Point other)
        {
            int dx = Math.Abs(other.X - X);
            int dy = Math.Abs(other.Y - Y);

            if (dx + dy > 2)
                return dx + dy;

            return Math.Max(dx, dy);
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

            // Kiinduló és cél mezők ellenőrzése, hogy a játéktábla tartományán belül vannak-e
            if (!IsWithinBounds(start, cells) || !IsWithinBounds(destination, cells))
            {
                return moveType;
            }

            // Kiinduló mező állapotának ellenőrzése
            if (cells[start.X, start.Y] != ownCellState)
            {
                return moveType;
            }

            // Cél mező állapotának ellenőrzése
            if (cells[destination.X, destination.Y] != CellState.Empty)
            {
                return moveType;
            }

            // Mozgás típusának ellenőrzése
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
