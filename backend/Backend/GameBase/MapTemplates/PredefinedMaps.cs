using AI.Abstractions;
using Backend.GameBase.Entities;

namespace Backend.GameBase.MapTemplates
{
    public static class PredefinedMaps
    {
        public static readonly CellState[,] SmallMap = new CellState[(int)BoardSize.Small, (int)BoardSize.Small]
        {
            { CellState.Player1, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Player2 }
        };

        public static readonly CellState[,] MediumMap = new CellState[(int)BoardSize.Medium, (int)BoardSize.Medium]
        {
            { CellState.Player1, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Player2 }
        };

        public static readonly CellState[,] LargeMap = new CellState[(int)BoardSize.Large, (int)BoardSize.Large]
        {
            { CellState.Player1, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty, CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty, CellState.Empty, CellState.Wall,  CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty },
            { CellState.Empty,   CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Empty, CellState.Player2 }
        };
    };
}
