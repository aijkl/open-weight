using OpenWeight.Commands;
using Spectre.Console.Cli;

namespace OpenWeight
{
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
}