using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Dreamland.Json;
using Dreamland.iOS;
using UnityEngine;
using UnityEngine.iOS;

namespace Dreamland
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private DreamlandConfig config;
        [SerializeField] private bool autoStart = true;
        [SerializeField] private int statusPollAttempts = 20;
        [SerializeField] private float statusPollIntervalSeconds = 2f;
        [SerializeField] private float captureDurationSeconds = 10f;

        private DreamlandApiClient apiClient;
        private RoomBundleLoader bundleLoader;
        private TelemetryClient telemetry;

        void Awake()
        {
            EnsureRoomPlanReceiver();
            apiClient = gameObject.AddComponent<DreamlandApiClient>();
            bundleLoader = gameObject.AddComponent<RoomBundleLoader>();
            telemetry = gameObject.AddComponent<TelemetryClient>();

            apiClient.Configure(config);
            telemetry.Configure(config);
        }

        void Start()
        {
            if (autoStart)
            {
                StartCoroutine(RunInternalBetaFlow());
            }
        }

        private void EnsureRoomPlanReceiver()
        {
            var existing = GameObject.Find("RoomPlanBridge");
            if (existing != null)
            {
                return;
            }
            var go = new GameObject("RoomPlanBridge");
            go.AddComponent<RoomPlanCaptureReceiver>();
        }

        private IEnumerator RunInternalBetaFlow()
        {
            if (!IsCaptureSupported())
            {
                Debug.LogError("RoomPlan not supported on this device.");
                yield break;
            }

            yield return RequestCameraPermission();

            yield return telemetry.Emit("scan_start", new Dictionary<string, object>());

            string scanId = null;
            string manifestUploadUrl = null;
            string usdzUploadUrl = null;
            string jsonUploadUrl = null;

            var sessionPayload = new Dictionary<string, object>
            {
                { "user_id", config.userId },
                { "room_category", "bedroom" },
                { "privacy_tier", "private" },
                { "artifacts", new object[]
                    {
                        new Dictionary<string, object> { { "name", "room.usdz" } },
                        new Dictionary<string, object> { { "name", "room.json" } }
                    }
                }
            };

            var sessionJson = MiniJsonSerialize(sessionPayload);
            yield return apiClient.PostJson("/scans/session", sessionJson, (request) =>
            {
                if (request == null || request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Session error: " + request?.error);
                    return;
                }

                var data = MiniJson.Deserialize(request.downloadHandler.text) as Dictionary<string, object>;
                if (data == null) return;
                scanId = data["scan_id"].ToString();

                var uploadUrls = data["upload_urls"] as Dictionary<string, object>;
                if (uploadUrls != null)
                {
                    manifestUploadUrl = uploadUrls["manifest"].ToString();
                    var artifacts = uploadUrls["artifacts"] as List<object>;
                    if (artifacts != null && artifacts.Count >= 2)
                    {
                        usdzUploadUrl = (artifacts[0] as Dictionary<string, object>)?["url"].ToString();
                        jsonUploadUrl = (artifacts[1] as Dictionary<string, object>)?["url"].ToString();
                    }
                }
            });

            if (string.IsNullOrEmpty(scanId) || string.IsNullOrEmpty(manifestUploadUrl) || string.IsNullOrEmpty(usdzUploadUrl) || string.IsNullOrEmpty(jsonUploadUrl))
            {
                Debug.LogError("Missing scan session data.");
                yield break;
            }

            RoomPlanCapture.StartCapture();
            yield return new WaitForSeconds(captureDurationSeconds);
            RoomPlanCapture.StopCapture();

            var export = RoomPlanCapture.ExportCapture();
            var usdzPath = export.usdzPath;
            var jsonPath = export.jsonPath;
            if (string.IsNullOrEmpty(usdzPath) || string.IsNullOrEmpty(jsonPath))
            {
                Debug.LogError("RoomPlan export failed.");
                yield break;
            }

            var manifestJson = ManifestBuilder.Build(scanId, config.userId, usdzPath, jsonPath);
            yield return apiClient.PutRaw(manifestUploadUrl, Encoding.UTF8.GetBytes(manifestJson), "application/json", null);
            yield return apiClient.PutFile(usdzUploadUrl, usdzPath, "model/vnd.usdz+zip", null);
            yield return apiClient.PutFile(jsonUploadUrl, jsonPath, "application/json", null);

            yield return telemetry.Emit("scan_upload_complete", new Dictionary<string, object> { { "scan_id", scanId } });

            yield return apiClient.PostJson($"/scans/{scanId}/commit", "{}", (request) =>
            {
                if (request == null || request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Commit error: " + request?.error);
                }
            });

            string status = null;
            for (int i = 0; i < statusPollAttempts; i += 1)
            {
                yield return new WaitForSeconds(statusPollIntervalSeconds);
                yield return apiClient.GetJson($"/scans/{scanId}/status", (request) =>
                {
                    if (request == null || request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("Status error: " + request?.error);
                        return;
                    }
                    var data = MiniJson.Deserialize(request.downloadHandler.text) as Dictionary<string, object>;
                    if (data != null && data.TryGetValue("status", out var statusObj))
                    {
                        status = statusObj?.ToString();
                    }
                });

                if (status == "ready" || status == "failed")
                {
                    break;
                }
            }

            if (status != "ready")
            {
                Debug.LogError("Scan not ready.");
                yield break;
            }

            string roomId = null;
            yield return apiClient.GetJson($"/scans/{scanId}/room", (request) =>
            {
                var data = MiniJson.Deserialize(request.downloadHandler.text) as Dictionary<string, object>;
                if (data != null && data.TryGetValue("room_id", out var roomObj))
                {
                    roomId = roomObj?.ToString();
                }
            });

            if (string.IsNullOrEmpty(roomId))
            {
                Debug.LogError("Room not found.");
                yield break;
            }

            string manifestUrl = null;
            yield return apiClient.GetJson($"/rooms/{roomId}/bundle", (request) =>
            {
                var data = MiniJson.Deserialize(request.downloadHandler.text) as Dictionary<string, object>;
                if (data != null && data.TryGetValue("manifest_url", out var urlObj))
                {
                    manifestUrl = urlObj?.ToString();
                }
            });

            if (string.IsNullOrEmpty(manifestUrl))
            {
                Debug.LogError("Manifest URL missing.");
                yield break;
            }

            yield return telemetry.Emit("room_load_start", new Dictionary<string, object> { { "room_id", roomId } });

            RoomBundleManifest manifest = null;
            yield return bundleLoader.LoadBundle(manifestUrl,
                (loaded) => { manifest = loaded; },
                (error) => { Debug.LogError("Bundle load error: " + error); }
            );

            if (manifest == null)
            {
                yield break;
            }

            bool roomLoaded = false;
            yield return bundleLoader.LoadRoom(manifestUrl, manifest,
                (_root) => { roomLoaded = true; },
                (error) => { Debug.LogError("Room load error: " + error); }
            );

            if (!roomLoaded)
            {
                yield break;
            }

            yield return telemetry.Emit("room_load_success", new Dictionary<string, object> { { "room_id", roomId } });

            SpawnPlayer();
            yield return telemetry.Emit("movement_start", new Dictionary<string, object> { { "room_id", roomId } });
        }

        private bool IsCaptureSupported()
        {
#if UNITY_EDITOR
            return true;
#else
            var os = DeviceGate.GetOsVersion();
            var model = DeviceGate.GetDeviceModel();
            var hasLiDAR = DeviceGate.HasLiDAR(model);
            return DeviceGate.IsSupported(os, hasLiDAR);
#endif
        }

        private IEnumerator RequestCameraPermission()
        {
#if UNITY_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }
#endif
            yield return null;
        }

        private static string MiniJsonSerialize(Dictionary<string, object> obj)
        {
            return JsonUtility.ToJson(new SerializableManifest(obj));
        }

        private void SpawnPlayer()
        {
            var player = new GameObject("Player");
            player.transform.position = new Vector3(0f, 1f, 0f);
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.3f;
            player.AddComponent<PlayerController>();
        }

        [Serializable]
        private class SerializableManifest
        {
            public string scan_id;
            public string owner_user_id;
            public Capture capture;
            public Consent consent;
            public Artifact[] artifacts;

            public SerializableManifest(Dictionary<string, object> source)
            {
                scan_id = source["scan_id"].ToString();
                owner_user_id = source["owner_user_id"].ToString();
                capture = new Capture();
                consent = new Consent();
                artifacts = new[] { new Artifact(), new Artifact() };
            }
        }

        [Serializable]
        private class Capture
        {
            public string device_model = "iPhone";
            public string os_version = "iOS";
            public string app_version = "0.1.0";
            public int capture_duration_sec = 60;
            public string room_category = "bedroom";
            public string privacy_tier = "private";
        }

        [Serializable]
        private class Consent
        {
            public bool upload_scope = true;
            public bool extended_use_scope = false;
            public string consent_version = "2026-02";
        }

        [Serializable]
        private class Artifact
        {
            public string name = "room.usdz";
            public int bytes = 10;
            public string sha256 = "placeholder";
        }
    }
}
