using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Bodardr.Utility.Editor
{
    public static class SerializedPropertyExtensions
    {
        private const string arrayDataPath = ".Array.data";

        public static object GetValue(this SerializedProperty prop)
        {
            if (prop.propertyPath.EndsWith(']'))
                return GetArrayElementValue(prop);

            var splitPath = prop.propertyPath.Split('.');
            object obj = prop.serializedObject.targetObject;

            for (int i = 0; i < splitPath.Length; i++)
            {
                var field = obj.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Single(x => x.Name == splitPath[i]);

                obj = field.GetValue(obj);
            }

            return obj;
        }

        private static object GetArrayElementValue(SerializedProperty prop)
        {
            var path = prop.propertyPath;
            var indexOf = path.IndexOf(arrayDataPath);

            path = path.Remove(indexOf, arrayDataPath.Length);

            var splitPath = path.Split('.');
            object obj = prop.serializedObject.targetObject;

            for (int i = 0; i < splitPath.Length; i++)
            {
                var s = splitPath[i];

                if (s.IndexOf('[') >= 0)
                    s = s.Remove(s.IndexOf('['));

                var field = obj.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Single(x => x.Name == s);

                obj = field.GetValue(obj);
            }

            var splitPathLast = splitPath[^1];
            var indexOfStart = splitPathLast.IndexOf('[');
            var index = int.Parse(splitPathLast.Substring(indexOfStart + 1, splitPathLast.Length - indexOfStart - 2));

            return ((Array)obj).GetValue(index);
        }

        public static SerializedProperty FindParent(this SerializedProperty prop)
        {
            if (prop.depth < 1)
            {
                throw new Exception("Cannot use 'GetParent' on root property");
            }

            var propPath = prop.propertyPath;
            return prop.serializedObject.FindProperty(propPath[..propPath.LastIndexOf('.')]);
        }

        public static SerializedProperty FindSiblingProperty(this SerializedProperty prop, string relativePropertyPath)
        {
            return prop.depth > 0
                ? prop.FindParent().FindPropertyRelative(relativePropertyPath)
                : prop.serializedObject.FindProperty(relativePropertyPath);
        }
    }
}