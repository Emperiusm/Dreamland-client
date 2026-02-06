using System;
using UnityEngine;

namespace Dreamland.iOS
{
    public static class DeviceGate
    {
        private static readonly string[] LiDARiPhoneModels = {
            "iphone 12 pro",
            "iphone 12 pro max",
            "iphone 13 pro",
            "iphone 13 pro max",
            "iphone 14 pro",
            "iphone 14 pro max",
            "iphone 15 pro",
            "iphone 15 pro max"
        };

        private static readonly string[] LiDARiPadModels = {
            "ipad pro"
        };

        public static bool IsSupported(string osVersion, bool hasLiDAR)
        {
            if (!hasLiDAR)
            {
                return false;
            }

            if (!TryParseMajorVersion(osVersion, out var major))
            {
                return false;
            }

            return major >= 16;
        }

        public static bool TryParseMajorVersion(string osVersion, out int major)
        {
            major = 0;
            if (string.IsNullOrEmpty(osVersion))
            {
                return false;
            }

            var digits = string.Empty;
            foreach (var ch in osVersion)
            {
                if (char.IsDigit(ch))
                {
                    digits += ch;
                }
                else if (digits.Length > 0)
                {
                    break;
                }
            }

            return int.TryParse(digits, out major);
        }

        public static bool HasLiDAR(string deviceModel)
        {
            if (string.IsNullOrEmpty(deviceModel))
            {
                return false;
            }

            var model = deviceModel.ToLowerInvariant();
            foreach (var entry in LiDARiPhoneModels)
            {
                if (model.Contains(entry))
                {
                    return true;
                }
            }
            foreach (var entry in LiDARiPadModels)
            {
                if (model.Contains(entry))
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetOsVersion()
        {
            return SystemInfo.operatingSystem;
        }

        public static string GetDeviceModel()
        {
            return SystemInfo.deviceModel;
        }
    }
}
