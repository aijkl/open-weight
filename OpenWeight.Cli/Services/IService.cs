using WeightScaleReceiver;

namespace Aijkl.OpenWeight.Services;

internal interface IService
{
    internal string Id { init; get; }
    internal void Execute(WeightData weightData);
}