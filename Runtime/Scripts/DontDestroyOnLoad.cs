using System;
using UnityEngine;

namespace Bodardr.UI.Runtime
{
    public class DontDestroyOnLoad<T> : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    CreateInstance();

                return instance;
            }
        }

        private static void CreateInstance()
        {
            GameObject go = new GameObject(typeof(T).Name, typeof(T));
            instance = go.GetComponent<T>();
            DontDestroyOnLoad(go);
        }

        private void OnDestroy()
        {
            instance = default;
        }
    }
}