using UnityEngine;
using UnityEditor;

namespace UnitySteer.Base.Editors
{

[CustomEditor(typeof(DetectableObject))]
public class DetectableObjectEditor: Editor 
{
    /// <summary>
    /// Custom editor for the DetectableObjects. It will use the property 
    /// accesors for setting the radius and center, so that the dependent
    /// pre-calculated values are assigned as well.
    /// </summary>
	public override void OnInspectorGUI() 
    {
		var detectable = target as DetectableObject;
	    if (detectable == null) return;

	    var newCenter = EditorGUILayout.Vector3Field("Center", detectable.Center);
		if (newCenter != detectable.Center) 
		{
			detectable.Center = newCenter;
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
