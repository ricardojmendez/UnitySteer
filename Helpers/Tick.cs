using UnityEngine;
using System.Collections;

namespace UnitySteer.Helpers
{
	[System.Serializable]
	public class Tick {
		/// <summary>
		/// Next time the object is expected to be ticked
		/// </summary>
		float _nextTick = 0;
		
		[SerializeField]
		float _randomRangeMin = 0;
		[SerializeField]
		float _randomRangeMax = 0;
		
		/// <summary>
		/// How many seconds should pass before the object is ticked again
		/// </summary>
		[SerializeField]
		float _tickLapse = 0.1f;
		
		/// <summary>
		/// Tick priority (higher goes first)
		/// </summary>
		public float Priority = 0;
		
		
		/// <summary>
		/// Next time the object is expected to be ticked
		/// </summary>
		public float NextTick {
			get {
				return this._nextTick;
			}
		}	
		
		/// <summary>
		/// Allows us to set a range in which the next tick's time is randomized
		/// </summary>
		public float RandomRangeMax {
			get {
				return this._randomRangeMax;
			}
			set {
				_randomRangeMax = Mathf.Max(value, _randomRangeMin);
			}
		}

		/// <summary>
		/// Allows us to set a range in which the next tick's time is randomized
		/// </summary>
		public float RandomRangeMin {
			get {
				return this._randomRangeMin;
			}
			set {
				_randomRangeMin = Mathf.Min(value, _randomRangeMax);
			}
		}
		

		/// <summary>
		/// How many seconds should pass before the object is ticked again
		/// </summary>
		/// <remarks>
		/// Setting the TickLapse value will reset the next tick value.
		/// </remarks>
		public float TickLapse 
		{
			get {
				return this._tickLapse;
			}
			set {
				_tickLapse = Mathf.Max(value, 0);
				_nextTick = 0;
			}
		}

		
		public Tick() : this(0.1f) {
			
		}
		
		public Tick(float tickLapse) {
			TickLapse = tickLapse;
		}
		

		/// <summary>
		/// Will return true if we need to tick the current behavior and set the next tick time
		/// </summary>
		public bool TickBehavior() 
		{
			return TickBehavior(TickLapse);
		}
		
		/// <summary>
		/// Will return true if we need to tick the current behavior and set the next tick time
		/// </summary>
		/// <returns>
		/// True if we should tick, or false otherwise
		/// </returns>
		/// <param name='tickLapseOverride'>
		/// Value used to override the object's TickLapse property for the 
		/// next tick, if the method returns true
		/// </param>
		public bool TickBehavior(float nextTickLapseOverride) 
		{
			var result = CanTick();
			if (result)
			{
				_nextTick = Time.time + nextTickLapseOverride + Random.Range(_randomRangeMin, _randomRangeMax);
			}
			return result;
		}
		
		/// <summary>
		/// Peeks into if we can tick the action, without affecting the next tick time
		/// </summary>
		/// <returns>
		/// True if we can tick it, false otherwise <see cref="System.Boolean"/>
		/// </returns>
		public bool CanTick()
		{
			return _nextTick < Time.time;			
		}
		
	}
}