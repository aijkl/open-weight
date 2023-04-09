using Spectre.Console;
using Spectre.Console.Cli;
using System.Net.NetworkInformation;

namespace Aijkl.OpenWeight.Settings;

public class DaemonSettings : CommandSettings
{
    public DaemonSettings(string deviceAddress, string adapterName)
    {
        DeviceAddress = deviceAddress;
        AdapterName = adapterName;
    }

    [CommandArgument(0, "[DEVICE_ADDRESS]")]
    public string DeviceAddress { set; get; }

    [CommandArgument(1, "[ADAPTER_NAME]")]
    public string AdapterName { set; get; }

    [CommandOption("--interval-ms")] 
    public int IntervalMs { set; get; } = 300;

    [CommandArgument(2, "[CONNECTION_STRING]")] 
    public string ConnectionString { set; get; }

    public override ValidationResult Validate()
    {
        if (!PhysicalAddress.TryParse(DeviceAddress.AsSpan(), out _)) return ValidationResult.Error("マックアドレスのフォーマットではありません");
        if (string.IsNullOrWhiteSpace(AdapterName)) return ValidationResult.Error("アダプター名は必須です");
        if (string.IsNullOrWhiteSpace(ConnectionString)) return ValidationResult.Error("ConnectionStringは必須です");
        return ValidationResult.Success();
    }
}