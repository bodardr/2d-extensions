using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bodardr.Utility.Runtime
{
    public static class ImageLoader
    {
        private static Dictionary<string, Sprite> loadedSprites = new();

        public static IEnumerator TryLoadImage(string url, Action<Sprite> onLoad)
        {
            if (loadedSprites.ContainsKey(url))
            {
                onLoad(loadedSprites[url]);
                yield break;
            }
            
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            req.timeout = 3;

            yield return req.SendWebRequest();

            Sprite sprite = null;
            if (req.result == UnityWebRequest.Result.Success)
            {
                var tex = ((DownloadHandlerTexture)req.downloadHandler).texture;
                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                loadedSprites.Add(url, sprite);
                onLoad(sprite);
            }
            
            req.Dispose();
        }
    }
}