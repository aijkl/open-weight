using WeightScaleReceiver;

namespace Test
{
    public class Fit8SParserTest
    {
        [Fact]
        public void Test()
        {
            var stableWeightData = Fit8SParser.Parse(new List<byte> { 0x02, 0x91, 0xda, 0x18, 0x5e, 0x89, 0xaa, 0xc0, 0xaa, 0x01, 0x02, 0x1b, 0x01, 0x00, 0x00, 0x01, 0x00, 0x03, 0x03, 0x00 });
            Assert.Equal(new WeightData((float)72.450, true), stableWeightData);

            var nonStableWeightData = Fit8SParser.Parse(new List<byte> { 0x02, 0x91, 0xda, 0x18, 0x5e, 0x89, 0xaa, 0xc0, 0xaa, 0x01, 0x02, 0x1b, 0x01, 0x00, 0x00, 0x00, 0x00, 0x03, 0x03, 0x00 });
            Assert.Equal(new WeightData((float)72.450, false), nonStableWeightData);
        }
    }
}