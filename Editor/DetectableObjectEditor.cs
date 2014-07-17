using UnityEngine;
using UnityEditor;

namespace UnitySteer.Base.Editors
{

[CustomEditor(typeof(DetectableObject))]
public class DetectableObjectEditor: Editor 
{
	Vector3FoldoutEditor centerEditor = new Vector3FoldoutEditor("Center");
	
	public override void OnInspectorGUI() 
    {
		var detectable = target as DetectableObject;
	    if (detectable == null) return;

		var newCenter = centerEditor.DrawEditor(detectable.Center);
		if (newCenter != detectable.Center) 
		{
			detectable.Center = newCenter; // To avoid triggering the debugger.
			EditorUtility.SetDirty(detectable);
		}
		
		
		var newRadius = EditorGUILayout.FloatField("Radius", detectable.Radius);
		if (!Mathf.Approximately(newRadius, detectable.Radius))
		{
			detectable.Radius = newRadius;
			EditorUtility.SetDirty(detectable);
		}
		
		// Show default inspector property editor
		DrawDefaultInspector();
	}
}

}
