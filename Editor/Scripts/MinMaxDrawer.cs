using UnityEditor;
using UnityEngine;

namespace Bodardr.Utility.Editor.Editor.Scripts
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute), true)]
    public class MinMaxDrawer : PropertyDrawer
    {
        private const float numberRatio = 0.2f;
        private const float spacing = 5;

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.Vector2 &&
                property.propertyType != SerializedPropertyType.Vector2Int)
            {
                EditorGUI.LabelField(position, $"Property {property.name} must be a Vector2 or Vector2Int");
                return;
            }

            var att = (MinMaxAttribute)attribute;

            var labelRect = new Rect(position)
            {
                width = EditorGUIUtility.labelWidth
            };
            EditorGUI.LabelField(labelRect, label);

            position.x += labelRect.width;
            position.width -= labelRect.width;

            var numberWidth = position.width * numberRatio;
            
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var rect1 = new Rect(position)
            {
                width = numberWidth
            };

            position.x += rect1.width + spacing;
            var sliderRect = new Rect(position)
            {
                width = position.width - (numberWidth + spacing) * 2
            };

            position.x += sliderRect.width + spacing;
            var rect2 = new Rect(position)
            {
                width = numberWidth
            };

            EditorGUI.BeginChangeCheck();

            var isInt = property.propertyType == SerializedPropertyType.Vector2Int;

            Vector2 val = Vector2.zero;
            val = isInt ? property.vector2IntValue : property.vector2Value;

            if (isInt)
                val.x = EditorGUI.IntField(rect1, GUIContent.none, (int)val.x);
            else
                val.x = EditorGUI.FloatField(rect1, GUIContent.none, val.x);

            EditorGUI.MinMaxSlider(sliderRect, GUIContent.none, ref val.x, ref val.y, att.min, att.max);
            
            if (isInt)
                val.y = EditorGUI.IntField(rect2, GUIContent.none, (int)val.y);
            else
                val.y = EditorGUI.FloatField(rect2, GUIContent.none, val.y);

            if (EditorGUI.EndChangeCheck())
            {
                if (isInt)
                    property.vector2IntValue = Vector2Int.RoundToInt(val);
                else
                    property.vector2Value = val;
            }

            EditorGUI.indentLevel = indentLevel;
            EditorGUI.EndProperty();
        }
    }
}