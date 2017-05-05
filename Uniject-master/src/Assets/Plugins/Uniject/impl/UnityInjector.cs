using System;
using Ninject;
using Ninject.Injection;
using UnityEngine;
using Uniject.Impl;

public class UnityInjector
{
    private static IKernel kernel;
    
    public static IKernel get() {
        if (null == kernel) {
            kernel = new StandardKernel (new UnityNinjectSettings (), new Ninject.Modules.INinjectModule[] {
                new UnityModule (),
                new LateBoundModule(),
            } );
            
            GameObject listener = new GameObject();
            GameObject.DontDestroyOnLoad(listener);
            listener.name = "LevelLoadListener";
            listener.AddComponent<LevelLoadListener>();
        }
        return kernel;
    }
    
    public static object levelScope = new object();
    private static object scoper(Ninject.Activation.IContext context) {
        return levelScope;
    }
}

