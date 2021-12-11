using System;
using UnityEngine;

namespace Bodardr.UI.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string MemberName { get; }
        
        public bool Invert { get; }

        public ShowIfAttribute(string memberName, bool invert = false)
        {
            MemberName = memberName;
            Invert = invert;
        }
    }
}