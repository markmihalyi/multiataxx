using AI.Abstractions;
using Backend.GameBase.Serialization;
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
        public DbSet<UserStatistics> UserStatistics { get; set; }
        public DbSet<Booster> Boosters { get; set; }
        public DbSet<OwnedBooster> OwnedBoosters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new CellStateArrayConverter());
            jsonOptions.Converters.Add(new CellStateArrayListConverter());

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

            modelBuilder.Entity<OwnedBooster>()
                .HasKey(ob => new { ob.UserId, ob.BoosterId });
        }

    }
}
