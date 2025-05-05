using Backend.GameBase.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.GameBase.Serialization
{
    public class CellStateArrayListConverter : JsonConverter<List<CellState[,]>>
    {
        public override List<CellState[,]> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var rawList = JsonSerializer.Deserialize<List<List<List<CellState>>>>(ref reader, options);
            if (rawList == null) return [];

            var result = new List<CellState[,]>();

            foreach (var matrix in rawList)
            {
                int rows = matrix.Count;
                int cols = matrix[0].Count;
                var array = new CellState[rows, cols];

                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        array[i, j] = matrix[i][j];

                result.Add(array);
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, List<CellState[,]> value, JsonSerializerOptions options)
        {
            var serialized = new List<List<List<CellState>>>();

            foreach (var matrix in value)
            {
                var rows = matrix.GetLength(0);
                var cols = matrix.GetLength(1);
                var rowList = new List<List<CellState>>(rows);

                for (int i = 0; i < rows; i++)
                {
                    var row = new List<CellState>(cols);
                    for (int j = 0; j < cols; j++)
                        row.Add(matrix[i, j]);
                    rowList.Add(row);
                }

                serialized.Add(rowList);
            }

            JsonSerializer.Serialize(writer, serialized, options);
        }
    }

}
