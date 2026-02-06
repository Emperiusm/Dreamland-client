using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dreamland
{
    public static class ManifestBuilder
    {
        public static string Build(string scanId, string userId, string usdzPath, string jsonPath)
        {
            var artifacts = new List<Dictionary<string, object>>();

            if (File.Exists(usdzPath))
            {
                artifacts.Add(BuildArtifact("room.usdz", usdzPath));
            }

            if (File.Exists(jsonPath))
            {
                artifacts.Add(BuildArtifact("room.json", jsonPath));
            }

            var manifest = new Dictionary<string, object>
            {
                { "scan_id", scanId },
                { "owner_user_id", userId },
                { "capture", new Dictionary<string, object>
                    {
                        { "device_model", "iPhone" },
                        { "os_version", "iOS" },
                        { "app_version", "0.1.0" },
                        { "capture_duration_sec", 60 },
                        { "room_category", "bedroom" },
                        { "privacy_tier", "private" }
                    }
                },
                { "consent", new Dictionary<string, object>
                    {
                        { "upload_scope", true },
                        { "extended_use_scope", false },
                        { "consent_version", "2026-02" }
                    }
                },
                { "artifacts", artifacts.ToArray() }
            };

            return JsonUtility.ToJson(new SerializableManifest(manifest));
        }

        private static Dictionary<string, object> BuildArtifact(string name, string path)
        {
            var bytes = File.ReadAllBytes(path);
            return new Dictionary<string, object>
            {
                { "name", name },
                { "bytes", bytes.Length },
                { "sha256", ComputeSha256(bytes) }
            };
        }

        private static string ComputeSha256(byte[] bytes)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
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
                var list = source["artifacts"] as object[];
                artifacts = new Artifact[list != null ? list.Length : 0];
                if (list != null)
                {
                    for (int i = 0; i < list.Length; i++)
                    {
                        artifacts[i] = new Artifact(list[i] as Dictionary<string, object>);
                    }
                }
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
            public string name;
            public int bytes;
            public string sha256;

            public Artifact(Dictionary<string, object> source)
            {
                if (source == null)
                {
                    name = string.Empty;
                    bytes = 0;
                    sha256 = string.Empty;
                    return;
                }

                name = source["name"].ToString();
                bytes = Convert.ToInt32(source["bytes"]);
                sha256 = source["sha256"].ToString();
            }
        }
    }
}
