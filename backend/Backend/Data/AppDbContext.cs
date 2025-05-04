using Backend.GameLogic.Entities;
using Backend.GameLogic.Serialization;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Backend.Data
{
    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new CellStateArrayConverter());

            var converter = new ValueConverter<List<CellState[,]>, string>(
                v => JsonSerializer.Serialize(v, jsonOptions),
                v => JsonSerializer.Deserialize<List<CellState[,]>>(v, jsonOptions) ?? new()
            );

            var comparer = new ValueComparer<List<CellState[,]>>(
            (a, b) => JsonSerializer.Serialize(a, jsonOptions) == JsonSerializer.Serialize(b, jsonOptions),
                v => v == null ? 0 : JsonSerializer.Serialize(v, jsonOptions).GetHashCode(),
                v => JsonSerializer.Deserialize<List<CellState[,]>>(JsonSerializer.Serialize(v, jsonOptions), jsonOptions)!
            );

            modelBuilder.Entity<Match>()
                .Property(e => e.Steps)
                .HasConversion(converter)
                .Metadata.SetValueComparer(comparer);
        }

    }
}
