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
    private readonly string token;
    private Task? task;
    private readonly DiscordRestClient discordRestClient;
    private readonly CancellationTokenSource tokenSource;
    private readonly SemaphoreSlim semaphore;
    public DiscordService(DiscordSettings discordSettings)
    {
        token = discordSettings.Token;
        tokenSource = new CancellationTokenSource();
        discordRestClient = new DiscordRestClient();
        semaphore = new SemaphoreSlim(1);
        Users = discordSettings.GuildUsers;
        IntervalMs = discordSettings.IntervalMs;
        Queue = new Queue<QueueItem>();
    }
    public IList<GuildUser> Users { get; }
    public int IntervalMs { get; }
    public Queue<QueueItem> Queue { get; }
    public async Task StartAsync()
    {
        if (task != null) throw new InvalidOperationException("既に開始しています");

        await discordRestClient.LoginAsync(TokenType.Bot, token);
        AnsiConsoleHelper.MarkupLine($"{nameof(DiscordService)} Login", AnsiConsoleHelper.State.Success);

        task = Task.Run(async () =>
        {
            while (tokenSource.IsCancellationRequested is false)
            {
                try
                {
                    await semaphore.WaitAsync();

                    if(Queue.Any() is false) continue;

                    var queueItem = Queue.Dequeue();
                    try
                    {
                        var guild = await discordRestClient.GetGuildAsync(queueItem.GuildUser.GuildId);
                        var user = await guild.GetUserAsync(queueItem.GuildUser.UserId);
                        var nickName = await Template.Parse(queueItem.GuildUser.ScribanPattern).RenderAsync(new { queueItem.EventData });
                        await user.ModifyAsync(properties =>
                        {
                            properties.Nickname = nickName;
                        });

                        AnsiConsoleHelper.MarkupLine($"{nameof(DiscordService)}: {nickName}", AnsiConsoleHelper.State.Success);
                    }
                    catch (Exception exception)
                    {
                        Queue.Enqueue(queueItem);
                        AnsiConsole.WriteException(exception);
                    }
                }
                finally
                {
                    semaphore.Release();
                    await Task.Delay(IntervalMs, tokenSource.Token);
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