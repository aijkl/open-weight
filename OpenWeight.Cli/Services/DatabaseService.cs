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
    public void Execute(WeightDataEvent eventData)
    {
        if(eventData.Data.Stable is false) return;

        using (var context = new WeightContext(dbContextOptions))
        {
            context.Weights.Add(new WeightEntity
            {
                Timestamp = eventData.Timestamp,
                Weight = eventData.Data.Weight,
            });
            context.SaveChanges();
        }
        AnsiConsoleHelper.MarkupLine($"{nameof(DatabaseService)}: Inserted");
    }
}