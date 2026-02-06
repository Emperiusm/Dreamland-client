using System.IO;
using UnityEngine;

namespace Dreamland
{
    public static class AssetCache
    {
        public static string BuildKey(string bundleHash, string assetPath)
        {
            return bundleHash + "-" + assetPath.Replace('/', '_');
        }

        public static string GetCachePath(string key)
        {
            var dir = Path.Combine(Application.persistentDataPath, "dreamland-cache");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir, key);
        }

        public static bool TryGet(string bundleHash, string assetPath, out string cachedPath)
        {
            var key = BuildKey(bundleHash, assetPath);
            cachedPath = GetCachePath(key);
            return File.Exists(cachedPath);
        }

        public static void Store(string bundleHash, string assetPath, byte[] data)
        {
            var key = BuildKey(bundleHash, assetPath);
            var path = GetCachePath(key);
            File.WriteAllBytes(path, data);
        }
    }
}
