using UnityEngine;
using System.Collections;

public class ProximityDetection : MonoBehaviour {
    
    IAmVehicle vehicle;

	// Use this for initialization
	void Start () {
	    GameObject go = transform.parent.gameObject;
	    vehicle = go.GetComponent(typeof(IAmVehicle)) as IAmVehicle;
	    if (vehicle == null)
	        throw new System.Exception ("Vehicle component not found in parent");
	    // Debug.Log("Vehicle "+vehicle);
	}
	
	void OnTriggerEnter(Collider collider)
	{
	    HandleVisibility(collider.gameObject, true);
	}

	void OnTriggerExit(Collider collider)
	{
	    HandleVisibility(collider.gameObject, false);
	}
	
	void HandleVisibility(GameObject other, bool visible)
	{
	    // In theory the requested component should inherit from MonoBehavior,
	    // But this seems fine as is.  TODO-REVIEW
	    IAmVehicle otherV = other.GetComponent(typeof(IAmVehicle)) as IAmVehicle;
	    if (otherV == null)
	        return;
	    if (visible)
	        vehicle.Vehicle.Neighbors.Add(otherV.Vehicle);
	    else
	        vehicle.Vehicle.Neighbors.Remove(otherV.Vehicle);
	}
}
