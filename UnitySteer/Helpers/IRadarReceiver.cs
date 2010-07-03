using UnityEngine;

public interface IRadarReceiver
{
	void OnRadarEnter (Collider other, Radar sender);
	void OnRadarExit (Collider other, Radar sender);
	void OnRadarStay (Collider other, Radar sender);
}