using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Dreamland
{
    public class DreamlandApiClient : MonoBehaviour
    {
        [SerializeField] private DreamlandConfig config;

        public void Configure(DreamlandConfig newConfig)
        {
            config = newConfig;
        }

        public IEnumerator PostJson(string path, string json, Action<UnityWebRequest> callback)
        {
            var request = new UnityWebRequest(CombineUrl(path), "POST");
            var bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            AttachHeaders(request);
            yield return request.SendWebRequest();
            callback?.Invoke(request);
        }

        public IEnumerator GetJson(string path, Action<UnityWebRequest> callback)
        {
            var request = UnityWebRequest.Get(CombineUrl(path));
            AttachHeaders(request);
            yield return request.SendWebRequest();
            callback?.Invoke(request);
        }

        public IEnumerator PutRaw(string url, byte[] payload, string contentType, Action<UnityWebRequest> callback)
        {
            var request = new UnityWebRequest(url, "PUT");
            request.uploadHandler = new UploadHandlerRaw(payload);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", contentType);
            yield return request.SendWebRequest();
            callback?.Invoke(request);
        }

        private string CombineUrl(string path)
        {
            if (config == null) return path;
            return config.apiBaseUrl.TrimEnd('/') + "/" + path.TrimStart('/');
        }

        private void AttachHeaders(UnityWebRequest request)
        {
            if (config == null) return;
            request.SetRequestHeader("x-client-key", config.clientApiKey);
            request.SetRequestHeader("x-user-token", config.userToken);
            request.SetRequestHeader("x-session-id", config.sessionId);
            request.SetRequestHeader("x-device-tier", config.deviceTier);
            request.SetRequestHeader("x-privacy-context", config.privacyContext);
        }
    }
}
