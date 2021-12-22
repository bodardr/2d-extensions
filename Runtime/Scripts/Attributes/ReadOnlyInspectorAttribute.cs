using System;
using UnityEngine;

namespace Bodardr.Utility.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ReadOnlyInspectorAttribute : PropertyAttribute
    {
    }
}