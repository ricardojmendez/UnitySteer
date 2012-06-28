using System;
namespace UnitySteer.Helpers
{
	/// <summary>
	/// Generic class for raising steering events
	/// </summary>
	public class SteeringEvent<T>
	{
		
		#region Public properties
		/// <summary>
		/// Message action
		/// </summary>
		public string Action { get; set; }
		
		/// <summary>
		/// Parameter being passed (for instance, the vehicle)
		/// </summary>
		public T Parameter { get; set; }
		
		/// <summary>
		/// Steering object sending the message
		/// </summary>
		public Steering Sender { get; set; }
		#endregion
		
		/// <summary>
		/// Public constructor
		/// </summary>
		/// <param name="sender">
		/// Steering object sending the message <see cref="Steering"/>
		/// </param>
		/// <param name="action">
		/// Action <see cref="System.String"/>
		/// </param>
		/// <param name="parameter">
		/// Message parameter <see cref="T"/>
		/// </param>
		public SteeringEvent (Steering sender, string action, T parameter) {
			Sender = sender;
			Action = action;
			Parameter = parameter;
		}
		
	}
}

