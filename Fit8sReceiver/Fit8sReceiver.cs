using HashtagChris.DotNetBlueZ;
using HashtagChris.DotNetBlueZ.Extensions;
using WeightScaleReceiver;

namespace Aijkl.Fit8sReceiver;

public class Fit8sReceiver : IDisposable
{
    private readonly CancellationTokenSource tokenSource;
    private Task? task;
    public Fit8sReceiver(string deviceAddress, string adapterName, int intervalMs, Adapter adapter)
    {
        DeviceAddress = deviceAddress;
        AdapterName = adapterName;
        IntervalMs = intervalMs;
        tokenSource = new CancellationTokenSource();
        Adapter = adapter;
    }

    public event Action<WeightData>? Received;
    public event Action<WeightData>? StableReceived;
    public Adapter Adapter { get; private set; }
    public string DeviceAddress { get; }
    public string AdapterName { get; }
    public int IntervalMs { get; }
    public async Task StartAsync()
    {
        if (task != null) throw new InvalidOperationException("既に開始しています");

        Adapter = await BlueZManager.GetAdapterAsync(AdapterName);
        if (Adapter == null) throw new InvalidOperationException("アダプターが見つかりません");

        task = Thread();
    }
    private async Task Thread()
    {
        await Adapter.SetDiscoveryFilterAsync(new Dictionary<string, object>
        {
            { "Transport", "le" }
        });
        await Adapter.StartDiscoveryAsync();

        WeightData? preWeightData = null;
        WeightData? preStableWeightData = null;
        while (tokenSource.IsCancellationRequested is false)
        {
            try
            {
                var device = await Adapter.GetDeviceAsync(DeviceAddress);
                var manufacturerData = await device.GetManufacturerDataAsync();
                if (manufacturerData == null) continue;

                var weightData = Fit8SParser.Parse((byte[])manufacturerData.Values.First());
                if (preWeightData != weightData)
                {
                    if (weightData.Stable && (preStableWeightData != weightData || preWeightData?.Stable is false))
                    {
                        OnStableReceived(weightData);
                        preStableWeightData = weightData;
                    }
                    else
                    {
                        OnReceived(weightData);
                    }
                    preWeightData = weightData;
                }
            }
            catch (Exception) // MEMO 雑だけどどんな例外が出るか分からん
            {
                // ignored
            }
            finally
            {
                await Task.Delay(IntervalMs, tokenSource.Token);
            }
        }
    }
    protected virtual void OnReceived(WeightData obj)
    {
        Received?.Invoke(obj);
    }
    protected virtual void OnStableReceived(WeightData obj)
    {
        StableReceived?.Invoke(obj);
    }
    public void Dispose()
    {
        tokenSource.Dispose();
        task?.Dispose();
        Adapter.Dispose();
    }
    public static async Task<Fit8sReceiver> CreateInstanceAsync(string deviceAddress, string adapterName, int interval)
    {
        var adapter = await BlueZManager.GetAdapterAsync(adapterName);
        if (adapter != null)
        {
            var receiver = new Fit8sReceiver(deviceAddress, adapterName, interval, adapter);
            return receiver;
        }

        throw new InvalidOperationException("アダプターが見つかりません");
    }
}