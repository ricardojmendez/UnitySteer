using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SteerForNeighbors))]
public class SteerForNeighborsEditor: Editor {
	DegreeEditor angleEditor = new DegreeEditor("Angle");
	
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeInspector();
		var steer = target as SteerForNeighbors;
		steer.AngleDegrees = angleEditor.DrawEditor(steer.AngleDegrees);
		
		// Show default inspector property editor
		DrawDefaultInspector();
	}
}
