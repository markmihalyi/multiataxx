using Backend.GameLogic.Entities;
using Backend.GameLogic.Serialization;
using System.Text.Json.Serialization;

namespace Backend.GameLogic.Logic
{
    public class GameBoard
    {
        [JsonConverter(typeof(CellStateArrayConverter))]
        public CellState[,] Cells { get; }

        public int Size { get; }

        public GameBoard(BoardSize size)
        {
            Size = (int)size;
            Cells = new CellState[Size, Size];
            InitializeBoard();
        }

        private bool IsStartingPosition(int i, int j)
        {
            return (i == 0 && j == 0) || (i == Size - 1 && j == Size - 1);
        }

        private CellState GetStartingPlayer(int i, int j)
        {
            return (i == 0 && j == 0) ? CellState.Player1 : CellState.Player2;
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (IsStartingPosition(i, j))
                    {
                        Cells[i, j] = GetStartingPlayer(i, j);
                    }
                    else
                    {
                        Cells[i, j] = CellState.Empty;
                    }
                }
            }
        }

        public void PerformMove(Point start, Point destination, MoveType moveType, CellState ownCellState)
        {
            // Ugrás esetén a kiinduló mező legyen szabad
            if (moveType == MoveType.JUMP)
            {
                Cells[start.X, start.Y] = CellState.Empty;
            }

            // Cél mező elfoglalása
            Cells[destination.X, destination.Y] = ownCellState;

            // Cél mezővel szomszédos ellenséges területek elfoglalása (ha van)
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
        }

        public (bool, GameResult?) CheckIfGameIsOver()
        {
            int playerOneCellCount = 0;
            int playerTwoCellCount = 0;
            bool playerOneCanMove = false;
            bool playerTwoCanMove = false;

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    var cellState = Cells[i, j];

                    if (cellState == CellState.Player1)
                    {
                        playerOneCellCount++;
                        playerOneCanMove = playerOneCanMove || CanMove(i, j);
                    }
                    else if (cellState == CellState.Player2)
                    {
                        playerTwoCellCount++;
                        playerTwoCanMove = playerTwoCanMove || CanMove(i, j);
                    }

                    // Ha mindkét játékos tud lépni, akkor nincs értelme további ellenőrzésnek
                    if (playerOneCanMove && playerTwoCanMove)
                    {
                        return (false, null);
                    }
                }
            }

            // Játék vége állapotok ellenőrzése
            if (!playerTwoCanMove && playerOneCellCount > playerTwoCellCount)
            {
                return (true, GameResult.Player1Won);
            }

            if (!playerOneCanMove && playerOneCellCount < playerTwoCellCount)
            {
                return (true, GameResult.Player2Won);
            }

            if (!playerOneCanMove && !playerTwoCanMove && playerOneCellCount == playerTwoCellCount)
            {
                return (true, GameResult.Draw);
            }

            return (false, null);
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
