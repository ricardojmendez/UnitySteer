using UnityEngine;
using System.Collections;



public interface IRadarReceiver
{
	void OnRadarEnter( Collider other, Radar sender );
	void OnRadarExit( Collider other, Radar sender );
	void OnRadarStay( Collider other, Radar sender );
}



public class Radar : MonoBehaviour
{
	public GameObject target;
	
	private IRadarReceiver receiver;
	
	
	
	public void Awake()
	{
		if( target == null )
		{
			receiver = GetComponent( typeof( IRadarReceiver ) ) as IRadarReceiver;
			if( receiver == null )
			{
				target = transform.parent.gameObject;
			}
		}
		
		if (receiver == null)
		    receiver = target.GetComponent( typeof( IRadarReceiver ) ) as IRadarReceiver;
		
		if( receiver == null )
		{
			Debug.LogError( "No radar receiver. Self destruct in 3..." );
			Destroy( this );
		}
	}
	
	
	
	public void OnTriggerEnter( Collider other )
	{
		receiver.OnRadarEnter( other, this );
	}
	
	
	
	public void OnTriggerExit( Collider other )
	{
		receiver.OnRadarExit( other, this );
	}
	
	
	
	public void OnTriggerStay( Collider other )
	{
		receiver.OnRadarStay( other, this );
	}
}