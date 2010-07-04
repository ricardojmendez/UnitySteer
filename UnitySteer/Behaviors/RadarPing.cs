using System.Collections.Generic;
using UnityEngine;
using UnitySteer.Helpers;

/// <summary>
/// Detects neighbors by pinging an area with Physics.OverlapSphere
/// </summary>
public class RadarPing: Radar
{
	
	#region Private properties
	[SerializeField]
	private float _detectionRadius = 10;
	#endregion
	
	
	#region Public properties
	/// <summary>
	/// Radar ping detection radius
	/// </summary>
	public float DetectionRadius {
		get {
			return this._detectionRadius;
		}
		set {
			_detectionRadius = value;
		}
	}
	#endregion
	
	
	#region Methods
	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, _detectionRadius);
	}
	
	protected override IList<Collider> Detect()
	{
		var detected = Physics.OverlapSphere(transform.position, _detectionRadius, LayersChecked);
		var list = new List<Collider>(detected);
		
		/*
		foreach (var c in list)
		{
			Debug.DrawLine(transform.position, c.transform.position, Color.magenta);
		}
		*/
		return list;
	}
	#endregion
	
}