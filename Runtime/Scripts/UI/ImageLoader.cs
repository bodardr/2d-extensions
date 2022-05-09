using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Bodardr.Utility.Runtime
{
    public static class ImageLoader
    {
        private const int REQUEST_TIMEOUT = 30;
        
        private static Dictionary<string, Sprite> loadedSprites = new();

        /// <summary>
        /// Tries loading a web image from a URL,
        /// caches the result and calls an action to assign the loaded sprite.
        /// Since it uses caching : if it has been loaded, no web request
        /// will be created and the action will be called right away
        /// </summary>
        /// <param name="url">The web address of the image</param>
        /// <param name="onSpriteLoaded">The action called when the sprite has been loaded</param>
        /// <returns>This is a Unity Coroutine</returns>
        public static IEnumerator TryLoadImage(string url, Action<Sprite> onSpriteLoaded)
        {
            if (loadedSprites.ContainsKey(url))
            {
                onSpriteLoaded(loadedSprites[url]);
                yield break;
            }

            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            req.timeout = REQUEST_TIMEOUT;

            yield return req.SendWebRequest();

            Sprite sprite = null;
            if (req.result == UnityWebRequest.Result.Success)
            {
                var tex = ((DownloadHandlerTexture)req.downloadHandler).texture;
                sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                loadedSprites.Add(url, sprite);
                onSpriteLoaded(sprite);
            }

            req.Dispose();
        }

        /// <summary>
        /// - REQUIRED - The collection's type must have
        /// - a URL string
        /// - and a Sprite
        /// For the whole collection :
        /// Tries loading a web image from a URL,
        /// caches the result and calls an action to assign the loaded sprite.
        /// Since it uses caching : if it has been loaded, no web request
        /// will be created and the action will be called right away
        /// </summary>
        /// <param name="collection">The collection to fetch from</param>
        /// <param name="urlAccessor">The function performed on the collection to fetch the url</param>
        /// <param name="onSpriteLoaded">The action called when the sprite has been loaded</param>
        /// <typeparam name="T">The collection type</typeparam>
        /// <returns>This is a Unity Coroutine</returns>
        public static IEnumerator TryLoadImages<T>(IEnumerable<T> collection, Func<T, string> urlAccessor,
            Action<T, Sprite> onSpriteLoaded)
        {
            Dictionary<string, List<T>> dict = new Dictionary<string, List<T>>();
            List<Tuple<string, UnityWebRequest, UnityWebRequestAsyncOperation>> requests =
                new List<Tuple<string, UnityWebRequest, UnityWebRequestAsyncOperation>>();

            //Filling up dictionary with urls pointing to all the concerned items to be assigned to.
            foreach (var item in collection)
            {
                var url = urlAccessor(item);

                if (!dict.ContainsKey(url))
                    dict.Add(url, new List<T> { item });
                else
                    dict[url].Add(item);
            }

            //Creates Web Requests for all urls that aren't cached.
            foreach (var key in dict.Keys)
            {
                if (loadedSprites.ContainsKey(key))
                    continue;

                var request = UnityWebRequestTexture.GetTexture(key);
                request.timeout = REQUEST_TIMEOUT;
                var requestAsync = request.SendWebRequest();

                requests.Add(
                    new Tuple<string, UnityWebRequest, UnityWebRequestAsyncOperation>(key, request, requestAsync));
            }

            //Handle the creation of sprites and adding it to the loadedSprites static collection.
            foreach (var tuple in requests)
            {
                yield return tuple.Item3;

                var request = tuple.Item2;
                var url = tuple.Item1;

                Sprite sprite = null;
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    loadedSprites.Add(url, sprite);
                }
                
                request.Dispose();
            }

            foreach (var (key, value) in dict)
            {
                if (!loadedSprites.ContainsKey(key))
                    continue;
                
                var sprite = loadedSprites[key];
                foreach (var item in value)
                    onSpriteLoaded(item, sprite);
            }
        }
    }
}