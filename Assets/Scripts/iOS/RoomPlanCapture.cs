using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Dreamland.iOS
{
    public static class RoomPlanCapture
    {
        public enum CaptureState
        {
            Idle,
            Running,
            Completed,
            Failed
        }

        public static CaptureState State { get; private set; } = CaptureState.Idle;
        public static string LastUsdZPath { get; private set; } = string.Empty;
        public static string LastJsonPath { get; private set; } = string.Empty;
        public static string LastError { get; private set; } = string.Empty;

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
            State = CaptureState.Running;
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
            LastUsdZPath = usdz;
            LastJsonPath = json;
            return (usdz, json);
        }

        public static void UpdateState(string eventType, string usdzPath, string jsonPath, string error)
        {
            switch (eventType)
            {
                case "capture_started":
                    State = CaptureState.Running;
                    break;
                case "capture_completed":
                    State = CaptureState.Completed;
                    LastUsdZPath = usdzPath ?? string.Empty;
                    LastJsonPath = jsonPath ?? string.Empty;
                    break;
                case "capture_failed":
                    State = CaptureState.Failed;
                    LastError = error ?? string.Empty;
                    break;
            }
        }
    }
}
