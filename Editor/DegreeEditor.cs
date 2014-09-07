using UnityEditor;
using UnityEngine;

namespace UnitySteer.Editor
{
    /// <summary>
    /// Displays an editor field which takes degrees from the user and returns a cosine of the angle
    /// </summary>
    public class DegreeEditor
    {
        private string _label = "";

        public DegreeEditor(string label)
        {
            _label = label;
        }

        public float DrawEditor(float angle)
        {
            EditorGUILayout.BeginVertical();
            var newAngle = EditorGUILayout.FloatField(_label, angle);
            newAngle = Mathf.Clamp(newAngle, -360, 360);
            EditorGUILayout.EndVertical();
            return newAngle;
        }
    }
}