using System.Collections.Generic;
using Dreamland.Json;

namespace Dreamland
{
    public class RoomBundleManifest
    {
        public string bundleId;
        public string schemaVersion;
        public string pipelineVersion;
        public string roomId;
        public Dictionary<string, string> assets = new Dictionary<string, string>();
        public Dictionary<string, string> checksums = new Dictionary<string, string>();

        public static RoomBundleManifest Parse(string json)
        {
            var manifest = new RoomBundleManifest();
            var parsed = MiniJson.Deserialize(json) as Dictionary<string, object>;
            if (parsed == null) return manifest;

            manifest.bundleId = GetString(parsed, "bundle_id");
            manifest.schemaVersion = GetString(parsed, "schema_version");
            manifest.pipelineVersion = GetString(parsed, "pipeline_version");
            manifest.roomId = GetString(parsed, "room_id");

            if (parsed.TryGetValue("assets", out var assetsObj))
            {
                var assets = assetsObj as Dictionary<string, object>;
                if (assets != null)
                {
                    foreach (var pair in assets)
                    {
                        manifest.assets[pair.Key] = pair.Value?.ToString() ?? string.Empty;
                    }
                }
            }

            if (parsed.TryGetValue("checksums", out var checksumsObj))
            {
                var checksums = checksumsObj as Dictionary<string, object>;
                if (checksums != null)
                {
                    foreach (var pair in checksums)
                    {
                        manifest.checksums[pair.Key] = pair.Value?.ToString() ?? string.Empty;
                    }
                }
            }

            return manifest;
        }

        static string GetString(Dictionary<string, object> dict, string key)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value?.ToString() ?? string.Empty;
            }
            return string.Empty;
        }
    }
}
