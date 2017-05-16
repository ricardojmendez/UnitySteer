using UnityEditor;
using UnityEngine;
using UnitySteer.Attributes;

namespace UnitySteer.Editor
{
    [CustomPropertyDrawer(typeof (AngleCosineAttribute))]
    public class AngleCosineDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as AngleCosineAttribute;

            if (label.text.EndsWith("Cos"))
            {
                label.text = label.text.Substring(0, label.text.Length - 3);
            }

            var angle = EditorGUI.Slider(position, label, OpenSteerUtility.DegreesFromCos(property.floatValue), attr.Min,
                attr.Max);
            property.floatValue = OpenSteerUtility.CosFromDegrees(angle);
        }
    }
}