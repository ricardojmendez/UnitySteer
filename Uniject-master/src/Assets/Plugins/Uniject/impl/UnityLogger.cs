using System;

namespace Uniject.Impl {
    public class UnityLogger : ILogger{

        public string prefix { get; set; }

    	#region ILogger implementation
    	public void Log(string message) {
            if (null != prefix) {
                UnityEngine.Debug.Log(prefix);
            }
    		UnityEngine.Debug.Log(message);
    	}

        public void Log(string message, object[] args) {
            Log(string.Format(message, args));
        }
    	#endregion
    }
}
