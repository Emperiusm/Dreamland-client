using System;
using UnityEngine;

namespace Dreamland.iOS
{
    public static class DeviceGate
    {
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
            if (model.Contains("iphone") && model.Contains("pro"))
            {
                return true;
            }

            if (model.Contains("ipad") && model.Contains("pro"))
            {
                return true;
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
