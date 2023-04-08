using Aijkl.OpenWeight.Database;
using Aijkl.OpenWeight.Database.Entities;
using Aijkl.OpenWeight.Helpers;
using Microsoft.EntityFrameworkCore;
using WeightScaleReceiver;

namespace Aijkl.OpenWeight.Services;

public class DatabaseService : IService
{
    private readonly DbContextOptions dbContextOptions;
    public DatabaseService(string connectionString)
    {
        var optionBuilder = new DbContextOptionsBuilder();
        optionBuilder.UseSqlite(connectionString);
        dbContextOptions = optionBuilder.Options;
    }
    public string Id { get; init; } = "database";
    public void Execute(WeightData weightData)
    {
        if(weightData.Stable is false) return;

        using var context = new WeightContext(dbContextOptions);
        context.Weights.Add(new WeightEntity
        {
            Timestamp = weightData.Timestamp,
            Weight = weightData.Weight,
        });
        context.SaveChanges();

        AnsiConsoleHelper.MarkupLine($"{nameof(DatabaseService)}: Inserted");
    }
}