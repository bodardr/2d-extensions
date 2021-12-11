using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ShowIfEnumAttribute : PropertyAttribute
{
    public string MemberName { get; }
    public int EnumValue { get; }
    public bool Invert { get; }

    public ShowIfEnumAttribute(string memberName, int enumValue, bool invert = false)
    {
        MemberName = memberName;
        EnumValue = enumValue;
        Invert = invert;
    }
}