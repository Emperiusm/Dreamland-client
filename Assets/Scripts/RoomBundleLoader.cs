using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using GLTFast;

namespace Dreamland
{
    public class RoomBundleLoader : MonoBehaviour
    {
        public IEnumerator LoadBundle(string manifestUrl, Action<RoomBundleManifest> onLoaded, Action<string> onError)
        {
            var request = UnityWebRequest.Get(manifestUrl);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
                yield break;
            }

            var manifest = RoomBundleManifest.Parse(request.downloadHandler.text);
            onLoaded?.Invoke(manifest);
        }

        public IEnumerator DownloadAsset(string url, Action<byte[]> onLoaded, Action<string> onError)
        {
            var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
                yield break;
            }

            onLoaded?.Invoke(request.downloadHandler.data);
        }

        public IEnumerator LoadRoom(string manifestUrl, RoomBundleManifest manifest, Action<GameObject> onLoaded, Action<string> onError)
        {
            var roomRoot = new GameObject("RoomRoot");
            var meshUrl = ResolveAssetUrl(manifestUrl, GetAsset(manifest, "mesh_lod0"));
            var colliderUrl = ResolveAssetUrl(manifestUrl, GetAsset(manifest, "collision_mesh"));

            if (string.IsNullOrEmpty(meshUrl))
            {
                onError?.Invoke("Missing mesh_lod0 in manifest.");
                yield break;
            }

            var meshTask = LoadGltf(meshUrl, roomRoot.transform, "RoomMesh");
            while (!meshTask.IsCompleted)
            {
                yield return null;
            }
            if (!meshTask.Result)
            {
                onError?.Invoke("Failed to load room mesh.");
                yield break;
            }

            if (!string.IsNullOrEmpty(colliderUrl))
            {
                var colliderTask = LoadGltf(colliderUrl, roomRoot.transform, "RoomCollider");
                while (!colliderTask.IsCompleted)
                {
                    yield return null;
                }
                if (colliderTask.Result)
                {
                    AssignMeshColliders(roomRoot, "RoomCollider");
                }
            }

            onLoaded?.Invoke(roomRoot);
        }

        public GameObject BuildPlaceholderRoom(RoomBundleManifest manifest)
        {
            var root = new GameObject("RoomRoot");
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.transform.SetParent(root.transform, false);
            floor.transform.localScale = new Vector3(2f, 1f, 2f);
            floor.name = "Floor";
            return root;
        }

        public static string ResolveAssetUrl(string manifestUrl, string assetPath)
        {
            if (string.IsNullOrEmpty(manifestUrl) || string.IsNullOrEmpty(assetPath))
            {
                return string.Empty;
            }

            var baseIndex = manifestUrl.LastIndexOf("/manifest.json", StringComparison.Ordinal);
            if (baseIndex <= 0)
            {
                return string.Empty;
            }

            var baseUrl = manifestUrl.Substring(0, baseIndex);
            return baseUrl.TrimEnd('/') + "/" + assetPath.TrimStart('/');
        }

        private static string GetAsset(RoomBundleManifest manifest, string key)
        {
            if (manifest == null || manifest.assets == null)
            {
                return string.Empty;
            }

            if (manifest.assets.TryGetValue(key, out var value))
            {
                return value;
            }

            return string.Empty;
        }

        private async Task<bool> LoadGltf(string url, Transform parent, string name)
        {
            var import = new GltfImport();
            var success = await import.Load(url);
            if (!success)
            {
                return false;
            }

            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            import.InstantiateMainScene(root.transform);
            return true;
        }

        private void AssignMeshColliders(GameObject root, string colliderRootName)
        {
            var colliderRoot = root.transform.Find(colliderRootName);
            if (colliderRoot == null)
            {
                return;
            }

            foreach (var meshFilter in colliderRoot.GetComponentsInChildren<MeshFilter>())
            {
                var meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
            }
        }
    }
}
