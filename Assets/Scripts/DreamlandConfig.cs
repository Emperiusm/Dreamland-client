using UnityEngine;

namespace Dreamland
{
    [CreateAssetMenu(fileName = "DreamlandConfig", menuName = "Dreamland/Config")]
    public class DreamlandConfig : ScriptableObject
    {
        [Header("API")]
        public string apiBaseUrl = "http://localhost:8080";
        public string clientApiKey = "local-client-key";
        public string userToken = "internal-beta-token";
        public string userId = "00000000-0000-0000-0000-000000000001";

        [Header("Telemetry")]
        public string sessionId = "session-1";
        public string deviceTier = "A";
        public string privacyContext = "internal_beta";
    }
}
