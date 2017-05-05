using Ninject;
using System;
using Uniject;
using UnityEngine;

public class InjectionRoot : MonoBehaviour {

    public string typeToInstantiate;
    public void Start() {
        UnityInjector.get().Get(Type.GetType(typeToInstantiate), new Ninject.Parameters.IParameter[] { });
    }
}
