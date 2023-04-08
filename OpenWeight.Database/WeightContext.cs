using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Aijkl.OpenWeight.Database.Entities;

namespace Aijkl.OpenWeight.Database;

public sealed class WeightContext : DbContext
{
    public WeightContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<WeightEntity> Weights { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.LogTo(s => Debug.WriteLine(s));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);
    }

}