using System;

namespace Uniject.Impl {
    public class UnityLayerMask : ILayerMask {
    	public int NameToLayer(string name) {
    		return UnityEngine.LayerMask.NameToLayer(name);
    	}
    }
}
