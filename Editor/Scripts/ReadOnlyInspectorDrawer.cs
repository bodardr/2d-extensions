using Bodardr.Utility.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bodardr.Utility.Editor
{
    
        [CustomPropertyDrawer(typeof(ReadOnlyInspectorAttribute))]

    public class ReadOnlyInspectorDrawer : PropertyDrawer
    {
        private static GUIStyle richTextStyle;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + 16;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (richTextStyle == null)
                InitializeStyle();

            EditorGUI.LabelField(position, $"<b>{property.name}</b> : {property.GetValue()}", richTextStyle);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (richTextStyle == null)
                InitializeStyle();

            return new Label($"<b>{property.name}</b> : {property.GetValue()}"){enableRichText = true};
        }

        private void InitializeStyle()
        {
            richTextStyle = new GUIStyle { richText = true, normal = EditorStyles.label.normal, alignment = TextAnchor.MiddleCenter, fontSize = 16};
        }
    }
}