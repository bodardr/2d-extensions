using System;
using Random = UnityEngine.Random;

namespace Bodardr.Utility.Runtime
{
    public class EnumExtensions
    {
        public static TEnum RandomEnumValue<TEnum>() where TEnum : Enum
        {
            var values = Enum.GetValues(typeof(TEnum));
            var index = Random.Range(0, values.Length);
            return (TEnum)values.GetValue(index);
        }
    }
}