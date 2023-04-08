using Spectre.Console.Cli;
using Aijkl.OpenWeight.Settings;

namespace OpenWeight.Commands;

internal class DaemonCommand : AsyncCommand<DaemonSettings>
{
    public override Task<int> ExecuteAsync(CommandContext context, DaemonSettings settings)
    {
        throw new NotImplementedException();
    }
}