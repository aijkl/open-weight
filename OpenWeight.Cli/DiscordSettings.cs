using Aijkl.OpenWeight.Services;
using Newtonsoft.Json;

namespace Aijkl.OpenWeight;

public class DiscordSettings
{
    public DiscordSettings(string token, List<GuildUser> guildUsers)
    {
        Token = token;
        GuildUsers = guildUsers;
    }

    [JsonRequired]
    public string Token { set; get; }

    [JsonRequired]
    public List<GuildUser> GuildUsers { set; get; }

    [JsonRequired]
    public int IntervalMs { set; get; }

    [JsonRequired]
    public int ErrorIntervalMs { set; get; }
    public static DiscordSettings LoadFromFile(string filePath)
    {
        return JsonConvert.DeserializeObject<DiscordSettings>(File.ReadAllText(filePath)) ?? throw new ApplicationException();
    }
}