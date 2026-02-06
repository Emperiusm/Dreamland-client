using System.Runtime.InteropServices;
using UnityEngine;

namespace Dreamland.iOS
{
    public static class RoomPlanCapture
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void rp_start_capture();
        [DllImport("__Internal")] private static extern void rp_stop_capture();
        [DllImport("__Internal")] private static extern string rp_export_capture();
#else
        private static void rp_start_capture() {}
        private static void rp_stop_capture() {}
        private static string rp_export_capture() { return string.Empty; }
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

        public static string ExportCapture()
        {
            Debug.Log("[RoomPlan] ExportCapture");
            return rp_export_capture();
        }
    }
}
