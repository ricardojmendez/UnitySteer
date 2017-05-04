using System;
using UnityEngine;

namespace Uniject.Impl {

    /// <summary>
    /// Provider for UnityEngine.GameObject.
    /// We need this because we provide a binding for System.string,
    /// which causes Ninject to try to use the constructor that takes a string.
    /// </summary>
    public class GameObjectProvider : Ninject.Activation.Provider<GameObject> {
        protected override GameObject CreateInstance(Ninject.Activation.IContext context) {
            return new GameObject();
        }
    }
}
