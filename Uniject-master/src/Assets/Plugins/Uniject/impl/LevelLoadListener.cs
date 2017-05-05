using System;
using UnityEngine;

class LevelLoadListener : MonoBehaviour {
    
    public void Start() {
        DontDestroyOnLoad(this.gameObject);
    }
    
    public void OnLevelWasLoaded() {
        UnityInjector.levelScope = new object ();
    }
}
