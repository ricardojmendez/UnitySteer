using UnityEditor;
using UnityEngine;
using UnitySteer.Attributes;

namespace UnitySteer.Editor
{
    [CustomPropertyDrawer(typeof (Vector2ToggleAttribute))]
    public class Vector2TogglePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var current = property.vector2Value;

            position = EditorGUI.PrefixLabel(position, label);

            var oneHalf = Mathf.FloorToInt(position.width / 2);

            var xRect = new Rect(position.x, position.y, oneHalf, position.height);
            var yRect = new Rect(position.x + oneHalf, position.y, oneHalf, position.height);

            var onX = EditorGUI.ToggleLeft(xRect, "X", current.x == 1);
            var onY = EditorGUI.ToggleLeft(yRect, "Y", current.y == 1);

            current.x = onX ? 1 : 0;
            current.y = onY ? 1 : 0;

            property.vector2Value = current;
        }
    }
}