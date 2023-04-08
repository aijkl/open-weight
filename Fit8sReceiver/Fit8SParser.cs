namespace WeightScaleReceiver
{
    public static class Fit8SParser
    {
        public static WeightData Parse(IList<byte> bytes)
        {
            var weightInt = (bytes[10] & 255) | ((bytes[11] & 255) << 8) | ((bytes[12] & 255) << 16);
            var stable = bytes[15] == 1;
            return new WeightData((float)weightInt / 1000, stable, DateTime.Now);
        }
    }
}
