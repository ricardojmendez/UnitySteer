using System;

namespace Uniject.Impl {
    public class ResourceProvider<T> : Ninject.Activation.Provider<T> where T : UnityEngine.Object {

        private IResourceLoader loader;
        public ResourceProvider(IResourceLoader loader) {
            this.loader = loader;
        }

        protected override T CreateInstance(Ninject.Activation.IContext context) {
            Resource resource = Scoping.getContextAttribute<Resource>(context);
            if (resource == null) {
                throw new ArgumentException("Injected resources must have Resource attributes");
            }

            return loader.loadResource<T>(resource.Path);
        }
    }
}

