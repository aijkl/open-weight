using Spectre.Console.Cli;
using OpenWeight.Commands;

namespace Aijkl.OpenWeight;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var commandApp = new CommandApp();
        commandApp.Configure(x =>
        {
            x.AddCommand<DaemonCommand>("daemon");
        });
        return await commandApp.RunAsync(args);
    }
}