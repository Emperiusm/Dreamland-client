using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Dreamland.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Dreamland
{
    public class TelemetryClient : MonoBehaviour
    {
        [SerializeField] private DreamlandConfig config;

        public void Configure(DreamlandConfig newConfig)
        {
            config = newConfig;
        }

        public IEnumerator Emit(string eventName, Dictionary<string, object> payload)
        {
            if (config == null)
            {
                yield break;
            }

            var body = new Dictionary<string, object>
            {
                { "event_name", eventName },
                { "event_version", "v1" },
                { "timestamp", DateTime.UtcNow.ToString("o") },
                { "session_id", config.sessionId },
                { "device_tier", config.deviceTier },
                { "user_id", config.userId },
                { "privacy_context", config.privacyContext },
                { "payload", payload }
            };

            var json = JsonStringify(body);
            var request = new UnityWebRequest(config.apiBaseUrl.TrimEnd('/') + "/events", "POST");
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-client-key", config.clientApiKey);
            request.SetRequestHeader("x-user-token", config.userToken);
            yield return request.SendWebRequest();
        }

        private static string JsonStringify(Dictionary<string, object> obj)
        {
            return JsonUtility.ToJson(new SerializableWrapper(obj));
        }

        [Serializable]
        private class SerializableWrapper
        {
            public string event_name;
            public string event_version;
            public string timestamp;
            public string session_id;
            public string device_tier;
            public string user_id;
            public string privacy_context;
            public PayloadWrapper payload;

            public SerializableWrapper(Dictionary<string, object> source)
            {
                event_name = source["event_name"].ToString();
                event_version = source["event_version"].ToString();
                timestamp = source["timestamp"].ToString();
                session_id = source["session_id"].ToString();
                device_tier = source["device_tier"].ToString();
                user_id = source["user_id"].ToString();
                privacy_context = source["privacy_context"].ToString();
                payload = new PayloadWrapper(source["payload"] as Dictionary<string, object> ?? new Dictionary<string, object>());
            }
        }

        [Serializable]
        private class PayloadWrapper
        {
            public string scan_id;
            public string room_id;
            public string status;

            public PayloadWrapper(Dictionary<string, object> source)
            {
                if (source.TryGetValue("scan_id", out var scan)) scan_id = scan?.ToString();
                if (source.TryGetValue("room_id", out var room)) room_id = room?.ToString();
                if (source.TryGetValue("status", out var stat)) status = stat?.ToString();
            }
        }
    }
}
