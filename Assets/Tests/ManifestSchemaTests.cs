using NUnit.Framework;
using System.IO;
using Dreamland;

namespace Dreamland.Tests
{
    public class ManifestSchemaTests
    {
        [Test]
        public void FixtureContainsRequiredFields()
        {
            var json = File.ReadAllText("Assets/Tests/Fixtures/bundle_manifest.json");
            var manifest = RoomBundleManifest.Parse(json);
            Assert.IsNotEmpty(manifest.bundleId);
            Assert.IsNotEmpty(manifest.assets["mesh_lod0"]);
        }
    }
}
