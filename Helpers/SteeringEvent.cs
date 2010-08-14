using System;
namespace UnitySteer.Helpers
{
	#region Delegate declarations
	/// <summary>
	/// Delegate for steering event notifications
	/// </summary>
	/// <remarks>
	/// We really need the ability to communicate to whatever gameObject owns the
	/// vehicle that some steering action is considered complete, for instance
	/// when the pursuit behavior has reached its quarry.  I consider an event
	/// on the component a lot cleaner than having the main behavior polling
	/// constantly for "are we there yet?", particularly on cases like this one
	/// where events are so easily identifiable.
	/// </remarks>
	public delegate void SteeringEventHandler<T>(SteeringEvent<T> e);
	#endregion	
	
	/// <summary>
	/// Generic class for raising steering events
	/// </summary>
	public class SteeringEvent<T>
	{
		#region Internal members
		Steering _sender;
		string   _action;
		T       _parameter;
		#endregion
		
		#region Public properties
		public string Action {
			get {
				return this._action;
			}
			set {
				_action = value;
			}
		}

		public T Parameter {
			get {
				return this._parameter;
			}
			set {
				_parameter = value;
			}
		}

		public Steering Sender {
			get {
				return this._sender;
			}
			set {
				_sender = value;
			}
		}
		#endregion
		
		public SteeringEvent (Steering sender, string action, T parameter) {
			_sender = sender;
			_action = action;
			_parameter = parameter;
		}
		
	}
}

