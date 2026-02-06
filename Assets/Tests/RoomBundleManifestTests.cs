using NUnit.Framework;
using Dreamland;

namespace Dreamland.Tests
{
    public class RoomBundleManifestTests
    {
        [Test]
        public void ParsesBasicFields()
        {
            var json = "{" +
                       "\"bundle_id\":\"bundle123\"," +
                       "\"schema_version\":\"1.0\"," +
                       "\"pipeline_version\":\"proc\"," +
                       "\"room_id\":\"room1\"," +
                       "\"assets\":{\"mesh_lod0\":\"mesh/room.glb\"}" +
                       "}";

            var manifest = RoomBundleManifest.Parse(json);
            Assert.AreEqual("bundle123", manifest.bundleId);
            Assert.AreEqual("1.0", manifest.schemaVersion);
            Assert.AreEqual("proc", manifest.pipelineVersion);
            Assert.AreEqual("room1", manifest.roomId);
            Assert.AreEqual("mesh/room.glb", manifest.assets["mesh_lod0"]);
        }
    }
}
