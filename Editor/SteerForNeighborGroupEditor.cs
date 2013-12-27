using UnityEngine;
using UnityEditor;
using System.Collections;
using UnitySteer.Base;

namespace UnitySteer.Base.Editors
{

[CustomEditor(typeof(SteerForNeighborGroup))]
public class SteerForNeighborGroupEditor: Editor {
	DegreeEditor angleEditor = new DegreeEditor("Angle");
	
	public override void OnInspectorGUI() {
		EditorGUIUtility.LookLikeInspector();
		var steer = target as SteerForNeighborGroup;
		if (steer != null)
		{
			steer.AngleDegrees = angleEditor.DrawEditor(steer.AngleDegrees);
		}
		// Show default inspector property editor
		DrawDefaultInspector();
	}
}

}
