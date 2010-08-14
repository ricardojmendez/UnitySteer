using System;
namespace UnitySteer.Helpers
{
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

