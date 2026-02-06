using NUnit.Framework;
using System.IO;

namespace Dreamland.Tests
{
    public class InfoPlistTests
    {
        [Test]
        public void InfoPlistContainsCameraUsage()
        {
            var plist = File.ReadAllText("Assets/Plugins/iOS/Info.plist");
            Assert.IsTrue(plist.Contains("NSCameraUsageDescription"));
        }
    }
}
