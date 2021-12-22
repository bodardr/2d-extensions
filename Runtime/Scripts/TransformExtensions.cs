
    using Cinemachine;
    using UnityEngine;

    public static class TransformExtensions
    {
        public static Transform FindWithTag(this Transform tr, string tag)
        {
            foreach (Transform transform in tr)
            {
                if (transform.CompareTag(tag))
                    return transform;

                if (transform.childCount <= 0)
                    continue;
                
                var val = transform.FindWithTag(tag);
                    
                if (val)
                    return val;
            }

            return null;
        }
    }
