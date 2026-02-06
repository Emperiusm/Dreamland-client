using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Dreamland.iOS
{
    public static class RoomPlanCapture
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void rp_start_capture();
        [DllImport("__Internal")] private static extern void rp_stop_capture();
        [DllImport("__Internal")] private static extern IntPtr rp_export_capture_usdz();
        [DllImport("__Internal")] private static extern IntPtr rp_export_capture_json();
        [DllImport("__Internal")] private static extern void rp_free_string(IntPtr value);
#else
        private static void rp_start_capture() {}
        private static void rp_stop_capture() {}
        private static IntPtr rp_export_capture_usdz() { return IntPtr.Zero; }
        private static IntPtr rp_export_capture_json() { return IntPtr.Zero; }
        private static void rp_free_string(IntPtr value) {}
#endif

        public static void StartCapture()
        {
            Debug.Log("[RoomPlan] StartCapture");
            rp_start_capture();
        }

        public static void StopCapture()
        {
            Debug.Log("[RoomPlan] StopCapture");
            rp_stop_capture();
        }

        public static (string usdzPath, string jsonPath) ExportCapture()
        {
            Debug.Log("[RoomPlan] ExportCapture");
            var usdzPtr = rp_export_capture_usdz();
            var jsonPtr = rp_export_capture_json();
            var usdz = Marshal.PtrToStringAnsi(usdzPtr) ?? string.Empty;
            var json = Marshal.PtrToStringAnsi(jsonPtr) ?? string.Empty;
            rp_free_string(usdzPtr);
            rp_free_string(jsonPtr);
            return (usdz, json);
        }
    }
}
