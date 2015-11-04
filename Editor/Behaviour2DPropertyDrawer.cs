using UnityEditor;
using UnityEngine;
using UnitySteer.Attributes;

namespace UnitySteer.Editor
{
    [CustomPropertyDrawer(typeof(Behaviour2DAttribute))]
    public class Behaviour2DPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var current = property.stringValue;
            
            var text = property.displayName;

            var content = new GUIContent(text, current);

            var style = GUI.skin.label;

            style.alignment = TextAnchor.MiddleCenter;

            style.fontStyle = FontStyle.Bold;

            style.fixedHeight = 0f;

            EditorGUI.LabelField(position, content, style);
        }
    }
}