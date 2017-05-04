using System;
using Uniject;

namespace test {

    /// <summary>
    /// Acts as an equivalent to <c>UnityEngine.ResourceLoader</c> that verifies each loaded resource file exists.
    /// </summary>
    public class FileVerifyingResourceProvider<T>  : Ninject.Activation.Provider<T> where T : UnityEngine.Object {
        
        protected override T CreateInstance(Ninject.Activation.IContext context) {
            Resource resource = (Resource) context.Request.Target.GetCustomAttributes(typeof(Resource), false)[0];
            return (T) UnityEngine.Resources.Load(resource.Path);
        }
    }
}

