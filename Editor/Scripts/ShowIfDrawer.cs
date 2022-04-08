using Bodardr.UI.Runtime;
using Bodardr.Utility.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ShowIfAttribute), true)]
public class ShowIfDrawer : PropertyDrawer
{
    private bool show;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        UpdateShow(property);
        return show ? base.GetPropertyHeight(property, label) : 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        UpdateShow(property);
        
        if (show)
            EditorGUI.PropertyField(position, property, label);
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        UpdateShow(property);
        return show ? new PropertyField(property) : base.CreatePropertyGUI(property);
    }

    private void UpdateShow(SerializedProperty property)
    {
        var att = (ShowIfAttribute)attribute;
        
        var prop = property.FindSiblingProperty(att.MemberName);
        var s = prop.boolValue;

        if (att.Invert)
            s = !s;

        show = s;
    }
}