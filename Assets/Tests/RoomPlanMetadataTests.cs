using NUnit.Framework;
using Dreamland;

namespace Dreamland.Tests
{
    public class RoomPlanMetadataTests
    {
        [Test]
        public void MetadataParserExtractsRoomCategory()
        {
            var json = "{\"roomCategory\":\"bedroom\"}";
            var metadata = RoomPlanMetadata.Parse(json);
            Assert.AreEqual("bedroom", metadata.RoomCategory);
        }
    }
}
