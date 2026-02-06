using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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

        public GameObject BuildPlaceholderRoom(RoomBundleManifest manifest)
        {
            var root = new GameObject("RoomRoot");
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.transform.SetParent(root.transform, false);
            floor.transform.localScale = new Vector3(2f, 1f, 2f);
            floor.name = "Floor";
            return root;
        }
    }
}
