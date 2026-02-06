using System.Collections.Generic;
using Dreamland.Json;

namespace Dreamland
{
    public class RoomPlanMetadata
    {
        public string RoomCategory { get; private set; } = "bedroom";
        public int DurationSeconds { get; private set; } = 60;
        public string DeviceModel { get; private set; } = "iPhone";
        public string OsVersion { get; private set; } = "iOS";

        public static RoomPlanMetadata Parse(string json)
        {
            var metadata = new RoomPlanMetadata();
            var parsed = MiniJson.Deserialize(json) as Dictionary<string, object>;
            if (parsed == null)
            {
                return metadata;
            }

            if (parsed.TryGetValue("roomCategory", out var room))
            {
                metadata.RoomCategory = room?.ToString() ?? metadata.RoomCategory;
            }
            if (parsed.TryGetValue("duration", out var durationObj) && int.TryParse(durationObj.ToString(), out var duration))
            {
                metadata.DurationSeconds = duration;
            }
            if (parsed.TryGetValue("deviceModel", out var device))
            {
                metadata.DeviceModel = device?.ToString() ?? metadata.DeviceModel;
            }
            if (parsed.TryGetValue("osVersion", out var os))
            {
                metadata.OsVersion = os?.ToString() ?? metadata.OsVersion;
            }

            return metadata;
        }
    }
}
