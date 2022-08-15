using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Bodardr.Utility.Runtime
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Taken inspiration from : https://answers.unity.com/questions/863509/how-can-i-find-all-objects-that-have-a-script-that.html
        /// </summary>
        /// <typeparam name="T">The type of component to fetch</typeparam>
        /// <returns>All components across all scenes, containing this class or interface type.</returns>
        public static T[] FindComponentsOfTypeInAllScenes<T>()
        {
            List<T> components = new List<T>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                foreach (var rootGO in SceneManager.GetSceneAt(i).GetRootGameObjects())
                    components.AddRange(rootGO.GetComponentsInChildren<T>());
            }

            return components.ToArray();
        }
    }
}