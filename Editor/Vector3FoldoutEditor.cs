using UnityEditor;
using UnityEngine;

namespace UnitySteer.Editor
{
    /// <summary>
    /// Displays a vector 3 like the one used by the editor for MonoBehavior members
    /// </summary>
    /// <remarks>Seems that EditorGUILayout lacks a way to mimic the default behavior and
    /// only has Vector3Field. Added for visual consistency.</remarks>
    public class Vector3FoldoutEditor
    {
        public bool _foldout = false;
        private string _label = "";

        public Vector3FoldoutEditor(string label)
        {
            _label = label;
        }

        public Vector3 DrawEditor(Vector3 vector)
        {
            EditorGUILayout.BeginVertical();
            _foldout = EditorGUILayout.Foldout(_foldout, _label);

            if (_foldout)
            {
                var x = EditorGUILayout.FloatField("\t\tX", vector.x);
                var y = EditorGUILayout.FloatField("\t\tY", vector.y);
                var z = EditorGUILayout.FloatField("\t\tZ", vector.z);

                vector = new Vector3(x, y, z);
            }

            EditorGUILayout.EndVertical();

            return vector;
        }
    }
}