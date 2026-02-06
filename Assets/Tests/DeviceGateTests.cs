using NUnit.Framework;
using Dreamland.iOS;

namespace Dreamland.Tests
{
    public class DeviceGateTests
    {
        [Test]
        public void IsSupported_ReturnsFalse_ForUnsupportedOS()
        {
            var result = DeviceGate.IsSupported("15.0", true);
            Assert.IsFalse(result);
        }

        [Test]
        public void IsSupported_ReturnsFalse_WhenNoLiDAR()
        {
            var result = DeviceGate.IsSupported("16.0", false);
            Assert.IsFalse(result);
        }

        [Test]
        public void TryParseMajorVersion_ExtractsMajor()
        {
            Assert.IsTrue(DeviceGate.TryParseMajorVersion("iOS 16.2", out var major));
            Assert.AreEqual(16, major);
        }

        [Test]
        public void HasLiDAR_RecognizesProModels()
        {
            Assert.IsTrue(DeviceGate.HasLiDAR("iPhone 14 Pro"));
            Assert.IsTrue(DeviceGate.HasLiDAR("iPad Pro"));
            Assert.IsFalse(DeviceGate.HasLiDAR("iPhone 14"));
        }
    }
}
