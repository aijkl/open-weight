using Aijkl.Fit8sReceiver;
using Aijkl.OpenWeight;
using Aijkl.OpenWeight.Helpers;
using Aijkl.OpenWeight.Services;
using Spectre.Console.Cli;
using Aijkl.OpenWeight.Settings;
using Spectre.Console;

namespace OpenWeight.Commands;

public class DaemonCommand : AsyncCommand<DaemonSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DaemonSettings settings)
    {
        var services = new List<IService>();
        if (!string.IsNullOrWhiteSpace(settings.ConnectionString)) services.Add(new DatabaseService(settings.ConnectionString));
        if (!string.IsNullOrWhiteSpace(settings.DiscordSettingsPath)) services.Add(new DiscordService(DiscordSettings.LoadFromFile(settings.DiscordSettingsPath)));

        foreach (var service in services)
        {
            await service.StartAsync();
        }

        using var fit8SReceiver = await Fit8sReceiver.CreateInstanceAsync(settings.DeviceAddress, settings.AdapterName, settings.IntervalMs);
        fit8SReceiver.Received += eventData =>
        {
            AnsiConsoleHelper.MarkupLine($"Weight: {eventData.Data.Weight} Stable: {eventData.Data.Stable}");
        };
        fit8SReceiver.StableReceived += async eventData =>
        {
            foreach (var service in services)
            {
                try
                {
                    await service.ExecuteAsync(eventData);
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteException(ex);
                }
            }
            AnsiConsoleHelper.MarkupLine($"Weight: {eventData.Data.Weight}", AnsiConsoleHelper.State.Success);
        };
        await fit8SReceiver.StartAsync();

        await Task.Delay(-1);
        return 0;
    }
}