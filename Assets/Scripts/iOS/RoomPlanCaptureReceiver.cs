using System;
using UnityEngine;

namespace Dreamland.iOS
{
    public class RoomPlanCaptureReceiver : MonoBehaviour
    {
        [Serializable]
        private class CaptureEvent
        {
            public string type;
            public string usdz_path;
            public string json_path;
            public string error;
        }

        public void OnCaptureEvent(string payload)
        {
            var evt = JsonUtility.FromJson<CaptureEvent>(payload);
            if (evt == null || string.IsNullOrEmpty(evt.type))
            {
                return;
            }

            RoomPlanCapture.UpdateState(evt.type, evt.usdz_path, evt.json_path, evt.error);
        }
    }
}
