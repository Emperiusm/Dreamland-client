using NUnit.Framework;
using Dreamland;

namespace Dreamland.Tests
{
    public class RoomBundleLoaderTests
    {
        [Test]
        public void ResolvesAssetUrlFromManifestUrl()
        {
            var manifestUrl = "http://localhost:9000/processed/room/scan/bundle/hash/manifest.json";
            var assetPath = "mesh/room_lod0.glb";
            var expected = "http://localhost:9000/processed/room/scan/bundle/hash/mesh/room_lod0.glb";

            var actual = RoomBundleLoader.ResolveAssetUrl(manifestUrl, assetPath);
            Assert.AreEqual(expected, actual);
        }
    }
}
