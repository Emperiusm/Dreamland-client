using NUnit.Framework;
using Dreamland;

namespace Dreamland.Tests
{
    public class AssetCacheTests
    {
        [Test]
        public void CacheKeyIncludesBundleHash()
        {
            var key = AssetCache.BuildKey("bundle123", "mesh/room.glb");
            Assert.IsTrue(key.Contains("bundle123"));
        }
    }
}
