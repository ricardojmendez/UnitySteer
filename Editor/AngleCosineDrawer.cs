using UnityEngine;
using UnityEditor;
using UnitySteer;

[CustomPropertyDrawer(typeof(AngleCosineAttribute))]
public class AngleCosineDrawer: PropertyDrawer
{
	public override void OnGUI(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
	{
		var attr = attribute as AngleCosineAttribute;

		if (label.text.EndsWith("Cos"))
		{
			label.text = label.text.Substring(0, label.text.Length - 3);
		}

		var angle = EditorGUI.Slider(position, label, OpenSteerUtility.DegreesFromCos(property.floatValue), attr.Min, attr.Max);
		property.floatValue = OpenSteerUtility.CosFromDegrees(angle);
	}
}
