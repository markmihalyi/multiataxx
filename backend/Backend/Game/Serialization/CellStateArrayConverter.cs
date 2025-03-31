using Backend.GameLogic.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.GameLogic.Serialization
{
    public class CellStateArrayConverter : JsonConverter<CellState[,]>
    {
        public override CellState[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = JsonSerializer.Deserialize<List<List<CellState>>>(ref reader, options);
            if (list == null) return new CellState[0, 0];

            int rows = list.Count;
            int cols = list[0].Count;
            var array = new CellState[rows, cols];

            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    array[i, j] = list[i][j];

            return array;
        }

        public override void Write(Utf8JsonWriter writer, CellState[,] value, JsonSerializerOptions options)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);
            var list = new List<List<CellState>>(rows);

            for (int i = 0; i < rows; i++)
            {
                var row = new List<CellState>(cols);
                for (int j = 0; j < cols; j++)
                    row.Add(value[i, j]);
                list.Add(row);
            }

            JsonSerializer.Serialize(writer, list, options);
        }
    }
}
