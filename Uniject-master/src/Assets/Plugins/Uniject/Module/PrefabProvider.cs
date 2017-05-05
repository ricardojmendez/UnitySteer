using Ninject;
using System;
using Uniject;

/// <summary>
/// Denotes a parameter should be loaded as a Resource from a specified path.
/// Suitable for prefabs, audio clips etc.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Parameter)]
public class Resource : System.Attribute {
    public string Path { get; private set; }
    public Resource(string path) {
        this.Path = path;
    }
}

/// <summary>
/// A <c>Provider</c> that instantiates Unity prefabs wrapped as <c>TestableGameObject</c>.
/// </summary>
public class PrefabProvider : Ninject.Activation.Provider<TestableGameObject> {
    
    private IResourceLoader loader;
    public PrefabProvider(IResourceLoader loader) {
        this.loader = loader;
    }
    
    protected override TestableGameObject CreateInstance(Ninject.Activation.IContext context) {
        Resource attrib = (Resource) context.Request.Target.GetCustomAttributes(typeof(Resource), false)[0];
        return loader.instantiate(attrib.Path);
    }
}
