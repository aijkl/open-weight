using Spectre.Console;
using Spectre.Console.Cli;
using System.Net.NetworkInformation;

namespace Aijkl.OpenWeight.Settings;

public class DaemonSettings : CommandSettings
{
    public DaemonSettings(string deviceAddress)
    {
        DeviceAddress = deviceAddress;
    }

    [CommandArgument(0, "<DEVICE_ADDRESS>")]
    public string DeviceAddress { set; get; }

    public override ValidationResult Validate()
    {
        return !PhysicalAddress.TryParse(DeviceAddress.AsSpan(), out _) ? ValidationResult.Error("マックアドレスのフォーマットではありません") : ValidationResult.Success();
    }
}