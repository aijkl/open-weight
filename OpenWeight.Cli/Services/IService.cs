using WeightScaleReceiver;

namespace Aijkl.OpenWeight.Services;

internal interface IService
{
    internal string Id { init; get; }
    internal Task StartAsync()
    {
        return Task.CompletedTask;
    }
    internal Task ExecuteAsync(WeightDataEvent eventData);
}