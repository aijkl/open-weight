namespace WeightScaleReceiver;

public record WeightData(float Weight, bool Stable);
public record WeightDataEvent(WeightData Data, DateTime Timestamp);