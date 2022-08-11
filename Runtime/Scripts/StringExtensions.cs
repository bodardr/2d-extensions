using System;

namespace Bodardr.Utility.Runtime
{
    public static class StringExtensions
    {
        public static string RemoveCloneSuffix(this string str)
        {
            var indexOf = str.IndexOf("(Clone)", StringComparison.InvariantCulture);
            return str[..(indexOf - 1)];
        }
    }
}