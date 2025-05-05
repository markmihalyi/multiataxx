using Backend.GameBase.Entities;
using Backend.GameBase.MapTemplates;
using Backend.GameBase.Serialization;
using System.Text.Json.Serialization;

namespace Backend.GameBase.Logic
{
    public class GameBoard
    {
        [JsonConverter(typeof(CellStateArrayConverter))]
        public CellState[,] Cells { get; private set; } = new CellState[0, 0];
        public List<CellState[,]> Steps { get; } = [];

        public int Size { get; }

        public GameBoard(BoardSize size)
        {
            Size = (int)size;
            InitializeBoard();
            Steps.Add((CellState[,])Cells.Clone());
        }

        private void InitializeBoard()
        {
            switch (Size)
            {
                case (int)BoardSize.Small:
                    Cells = PredefinedMaps.SmallMap;
                    break;
                case (int)BoardSize.Medium:
                    Cells = PredefinedMaps.MediumMap;
                    break;
                case (int)BoardSize.Large:
                    Cells = PredefinedMaps.LargeMap;
                    break;
                default:
                    break;
            }
        }

        public void PerformMove(Point start, Point destination, MoveType moveType, CellState ownCellState)
        {
            // When jumping, the initial cell should be free
            if (moveType == MoveType.JUMP)
            {
                Cells[start.X, start.Y] = CellState.Empty;
            }

            // Capture target cell
            Cells[destination.X, destination.Y] = ownCellState;

            // Capture enemy cells next to the target cell (if any)
            CellState enemyCellState = ownCellState == CellState.Player1 ? CellState.Player2 : CellState.Player1;

            int firstCheckedRow = destination.X - 1;
            int lastCheckedRow = destination.X + 1;
            int firstCheckedColumn = destination.Y - 1;
            int lastCheckedColumn = destination.Y + 1;

            for (int i = firstCheckedRow; i <= lastCheckedRow; i++)
            {
                for (int j = firstCheckedColumn; j <= lastCheckedColumn; j++)
                {
                    if (IsValidCell(i, j) && Cells[i, j] == enemyCellState)
                    {
                        Cells[i, j] = ownCellState;
                    }
                }
            }

            Steps.Add((CellState[,])Cells.Clone());
        }

        public (bool, GameResult?) CheckIfGameIsOver()
        {
            // Checking whether players can still move
            bool playerOneCanMove = false;
            bool playerTwoCanMove = false;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    var cellState = Cells[i, j];

                    if (cellState == CellState.Player1)
                    {
                        playerOneCanMove = playerOneCanMove || CanMove(i, j);
                    }
                    else if (cellState == CellState.Player2)
                    {
                        playerTwoCanMove = playerTwoCanMove || CanMove(i, j);
                    }

                    // If both players can make a move, there is no need for further checks
                    if (playerOneCanMove && playerTwoCanMove)
                    {
                        return (false, null);
                    }
                }
            }

            // If one player can no longer move, the empty cells will belong to the other player
            if (playerOneCanMove && !playerTwoCanMove)
            {
                FillEmptyCells(CellState.Player1);
            }
            else if (!playerOneCanMove && playerTwoCanMove)
            {
                FillEmptyCells(CellState.Player2);
            }

            // Counts the number of cells captured by players
            int playerOneCellCount = 0;
            int playerTwoCellCount = 0;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    var cellState = Cells[i, j];

                    if (cellState == CellState.Player1)
                    {
                        playerOneCellCount++;
                    }
                    else if (cellState == CellState.Player2)
                    {
                        playerTwoCellCount++;
                    }
                }
            }

            // Calculate game result based on the number of cells captured
            if (playerOneCellCount > playerTwoCellCount)
            {
                return (true, GameResult.Player1Won);
            }

            if (playerOneCellCount < playerTwoCellCount)
            {
                return (true, GameResult.Player2Won);
            }

            return (true, GameResult.Draw);
        }

        public void FillEmptyCells(CellState cellState)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Cells[i, j] == CellState.Empty)
                    {
                        Cells[i, j] = cellState;
                    }
                }
            }
        }

        private bool IsValidCell(int x, int y) => x >= 0 && x < Size && y >= 0 && y < Size;

        private bool CanMove(int x, int y)
        {
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    int newX = x + dx;
                    int newY = y + dy;

                    if (IsValidCell(newX, newY) && Cells[newX, newY] == CellState.Empty)
                        return true;
                }
            }
            return false;
        }

        public void Debug()
        {
            for (int i = 0; i < Size; i++)
            {
                Console.Write("[ ");
                for (int j = 0; j < Size; j++)
                {
                    Console.Write((int)Cells[i, j] + " ");
                }
                Console.Write("]\n");
            }
        }
    }
}
