using System;

namespace Bodardr.Utility.Runtime
{
    public static class TypeExtensions
    {
        public static bool IsStaticType(this Type type)
        {
            if (type == null)
                return false;
            
            return type.IsAbstract && type.IsSealed;
        }
    }
}