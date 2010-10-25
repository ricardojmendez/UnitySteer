using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SteerForNeighborAvoidance))]
public class SteerForNeighborAvoidanceEditor: Editor {
	DegreeEditor avoidAngleEditor = new DegreeEditor("Avoidance angle");
	
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeInspector();
		var steer = target as SteerForNeighborAvoidance;
		steer.AvoidAngleDegrees = avoidAngleEditor.DrawEditor(steer.AvoidAngleDegrees);
		
		// Show default inspector property editor
		DrawDefaultInspector();
	}
}
