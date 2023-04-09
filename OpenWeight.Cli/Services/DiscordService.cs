using Discord;
using Scriban;
using Discord.Rest;
using Spectre.Console;
using WeightScaleReceiver;
using Aijkl.OpenWeight.Helpers;

namespace Aijkl.OpenWeight.Services;

public record GuildUser(ulong GuildId, ulong UserId, string ScribanPattern);
public record QueueItem(WeightDataEvent EventData, GuildUser GuildUser);
public class DiscordService : IService
{
    private readonly DiscordSettings _discordSettings;
    private Task? task;
    private readonly DiscordRestClient discordRestClient;
    private readonly CancellationTokenSource tokenSource;
    private readonly SemaphoreSlim semaphore;
    public DiscordService(DiscordSettings discordSettings)
    {
        _discordSettings = discordSettings;
        tokenSource = new CancellationTokenSource();
        discordRestClient = new DiscordRestClient();
        semaphore = new SemaphoreSlim(1);
        Users = discordSettings.GuildUsers;
        Queue = new Queue<QueueItem>();
    }
    public IList<GuildUser> Users { get; }
    public Queue<QueueItem> Queue { get; }
    public async Task StartAsync()
    {
        if (task != null) throw new InvalidOperationException("既に開始しています");

        await discordRestClient.LoginAsync(TokenType.Bot, _discordSettings.Token);
        AnsiConsoleHelper.MarkupLine($"{nameof(DiscordService)} Login", AnsiConsoleHelper.State.Success);

        task = Task.Run(async () =>
        {
            while (tokenSource.IsCancellationRequested is false)
            {
                try
                {
                    try
                    {
                        await semaphore.WaitAsync();

                        if (Queue.Any() is false)
                        {
                            await Task.Delay(100);
                            continue;
                        }


                        var queueItem = Queue.Peek();

                        var guild = await discordRestClient.GetGuildAsync(queueItem.GuildUser.GuildId);
                        var user = await guild.GetUserAsync(queueItem.GuildUser.UserId);
                        var nickName = await Template.Parse(queueItem.GuildUser.ScribanPattern).RenderAsync(new { queueItem.EventData });
                        await user.ModifyAsync(properties => { properties.Nickname = nickName; });

                        Queue.Dequeue();

                        AnsiConsoleHelper.MarkupLine($"{nameof(DiscordService)}: {nickName}", AnsiConsoleHelper.State.Success);
                    }
                    finally
                    {
                        semaphore.Release();
                    }

                    await Task.Delay(_discordSettings.IntervalMs, tokenSource.Token);
                }
                catch (Exception exception)
                {
                    AnsiConsole.WriteException(exception);
                    await Task.Delay(_discordSettings.ErrorIntervalMs, tokenSource.Token);
                }
            }
        });
    }
    public string Id { get; init; } = "discord";
    public async Task ExecuteAsync(WeightDataEvent eventData)
    {
        try
        {
            await semaphore.WaitAsync();

            Queue.Clear();
            foreach (var user in Users)
            {
                Queue.Enqueue(new QueueItem(eventData, user));
            }
        }
        finally
        {
            semaphore.Release();
        }
        AnsiConsoleHelper.MarkupLine($"{nameof(DiscordService)}: キューに登録しました");
    }

}