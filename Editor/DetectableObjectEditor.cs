using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DetectableObject))]
public class DetectableObjectEditor: Editor {
	Vector3FoldoutEditor centerEditor = new Vector3FoldoutEditor("Center");
	
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeInspector();
		var vehicle = target as DetectableObject;
		var newCenter = centerEditor.DrawEditor(vehicle.Center);
		if (newCenter != vehicle.Center) 
		{
			vehicle.Center = newCenter; // To avoid triggering the debugger.
			EditorUtility.SetDirty(vehicle);
		}
		
		
		var newRadius = EditorGUILayout.FloatField("\tRadius", vehicle.Radius);
		if (newRadius != vehicle.Radius)
		{
			vehicle.Radius = newRadius;
			EditorUtility.SetDirty(vehicle);
		}
		
		// Show default inspector property editor
		DrawDefaultInspector();
	}
}
