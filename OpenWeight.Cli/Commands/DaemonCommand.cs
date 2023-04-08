using Aijkl.Fit8sReceiver;
using Aijkl.OpenWeight.Helpers;
using Spectre.Console.Cli;
using Aijkl.OpenWeight.Settings;

namespace OpenWeight.Commands;

internal class DaemonCommand : AsyncCommand<DaemonSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, DaemonSettings settings)
    {
        var fit8SReceiver = await Fit8sReceiver.CreateInstanceAsync(settings.DeviceAddress, settings.AdapterName, settings.IntervalMs);
        fit8SReceiver.Received += data =>
        {
            AnsiConsoleHelper.MarkupLine($"Weight: {data.Weight} Stable: {data.Stable}");
        };
        fit8SReceiver.StableReceived += data =>
        {
            AnsiConsoleHelper.MarkupLine($"Weight: {data.Weight}", AnsiConsoleHelper.State.Success);
        };

        await fit8SReceiver.StartAsync();

        await Task.Delay(-1);
        return 0;
    }
}