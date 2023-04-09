using Aijkl.Fit8sReceiver;
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
        var services = new List<IService>
        {
            new DatabaseService(settings.ConnectionString)
        };

        var fit8SReceiver = await Fit8sReceiver.CreateInstanceAsync(settings.DeviceAddress, settings.AdapterName, settings.IntervalMs);
        fit8SReceiver.Received += eventData =>
        {
            AnsiConsoleHelper.MarkupLine($"Weight: {eventData.Data.Weight} Stable: {eventData.Data.Stable}");
        };
        fit8SReceiver.StableReceived += eventData =>
        {
            foreach (var service in services)
            {
                try
                {
                    service.Execute(eventData);
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