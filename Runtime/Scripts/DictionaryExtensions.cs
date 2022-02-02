using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DictionaryExtensions
{
    public static void Serialize<K,V>(this Dictionary<K,V> dict, out string[] keys, out string[] values)
    {
        SerializeCollection(dict.Keys, out keys);
        SerializeCollection(dict.Values, out values);
    }

    private static void SerializeCollection<T>(ICollection<T> collection, out string[] values)
    {
        values = new string[collection.Count];

        var i = 0;
        foreach (var key in collection)
        {
            values[i] = Serialize(key);
            i++;
        }
    }

    public static Dictionary<Key, Val> Deserialize<Key, Val>(string[] keysJson, string[] valuesJson)
    {
        var keys = Array.ConvertAll(keysJson, DeserializeValue<Key>);
        var values = Array.ConvertAll(valuesJson, DeserializeValue<Val>);

        return keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, y => y.v);
    }

    private static string Serialize<T>(T input)
    {
        var type = typeof(T);

        if (type == typeof(string))
            return input as string;

        return type.IsPrimitive ? input.ToString() : JsonUtility.ToJson(input);
    }

    private static T DeserializeValue<T>(string text)
    {
        var type = typeof(T);

        if (type == typeof(string))
            return (T)(object)text;

        if (type == typeof(bool))
            return (T)(object)string.Equals(text, "True", StringComparison.InvariantCultureIgnoreCase);

        return (T)(type.IsPrimitive ? Convert.ChangeType(text, type) : JsonUtility.FromJson(text, type));
    }
}