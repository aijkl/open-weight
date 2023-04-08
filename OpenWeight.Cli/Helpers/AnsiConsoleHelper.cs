using System.Diagnostics;
using Spectre.Console;

namespace Aijkl.OpenWeight.Helpers;

internal static class AnsiConsoleHelper
{
    internal enum State
    {
        Success,
        Failure,
        Info,
        Warning
    }
    private static readonly Dictionary<State, Color> ColorMap = new()
    {
        { State.Success, Color.Green1 },
        { State.Info, Color.Grey46 },
        { State.Failure, Color.Red1 },
        { State.Warning, Color.Yellow },
    };
    internal static void Log(string header, string headerColor, IEnumerable<(string Key, string Value)> messages, State state = State.Info)
    {
        MarkupLine($"[{headerColor}]{Spectre.Console.Markup.Escape(header)}[/] {string.Join(string.Empty, messages.Select(x => $" [gray]{Spectre.Console.Markup.Escape(x.Key)}[/]: {Spectre.Console.Markup.Escape(x.Value)}"))}", state);
    }
    internal static void Markup(string text, State state = State.Info)
    {
        if (!ColorMap.TryGetValue(state, out Color color)) return;
        AnsiConsole.Markup($" [[[rgb({color.R},{color.G},{color.B})]]]{state}[/] {text}");
    }
    internal static void MarkupLine(string text, State state = State.Info)
    {
        if (!ColorMap.TryGetValue(state, out Color color)) return;
        AnsiConsole.MarkupLine($" [[[rgb({color.R},{color.G},{color.B})]{state}[/]]] {text}");
    }
    internal static void WaitProgress(ProgressTask progressTask, int waitTimeMs)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();

        progressTask.Value = 0;
        while (stopwatch.ElapsedMilliseconds < waitTimeMs)
        {
            progressTask.Value = ((double)stopwatch.ElapsedMilliseconds / waitTimeMs) * 100;

            Thread.Sleep(50);
        }
        stopwatch.Stop();
        progressTask.Value = 100;
        progressTask.StopTask();
    }
}