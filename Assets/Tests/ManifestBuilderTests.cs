using NUnit.Framework;
using Dreamland;

namespace Dreamland.Tests
{
    public class ManifestBuilderTests
    {
        [Test]
        public void ManifestBuilder_IncludesUSDZArtifact()
        {
            var json = ManifestBuilder.Build("scan-id", "user-id", "room.usdz", "room.json");
            Assert.IsTrue(json.Contains("room.usdz"));
        }
    }
}
