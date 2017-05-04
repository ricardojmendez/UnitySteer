using System;

namespace Uniject {

    /// <summary>
    /// Moq based mock provider.
    /// Validates the paths of any resource attributes.
    /// </summary>
    public class MockProvider<T> : Ninject.Activation.Provider<T> where T : class {

        private IResourceLoader loader;
        public MockProvider(IResourceLoader loader) {
            this.loader = loader;
        }

        protected override T CreateInstance(Ninject.Activation.IContext context) {
            Resource resource = Scoping.getContextAttribute<Resource>(context);
            if (null != resource) {
                loader.loadResource<UnityEngine.Object>(resource.Path);
            }
            return new Moq.Mock<T>().Object;
        }
    }
}

