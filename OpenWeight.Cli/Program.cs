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
            x.AddCommand<DaemonCommand>("daemon").WithDescription("常時通信をして体重を表示します").WithExample(new[] { "AA:89:5E:18:DA:91", "hci0", "--interval 300" });
        });
        return await commandApp.RunAsync(args);
    }
}