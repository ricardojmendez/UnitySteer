using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using System;
using Uniject;
using Uniject.Configuration;

namespace Uniject.Impl {

    /// <summary>
    /// The last module to be bound.
    /// 
    /// This is designed to facilitate various iOS AOT workarounds.
    /// </summary>
    public class LateBoundModule : NinjectModule {

        public override void Load() {

            // We need these explicit instance bindings for AOT to work properly.
            Bind<string>().ToMethod(new XMLConfigProvider<string> (Kernel.Get<XMLConfigManager>()).CreateInstance);
            Bind<float>().ToMethod(new XMLConfigProvider<float> (Kernel.Get<XMLConfigManager>()).CreateInstance);
            Bind<double>().ToMethod(new XMLConfigProvider<double> (Kernel.Get<XMLConfigManager>()).CreateInstance);
        }

        private string provideString(IContext context) {
            return string.Empty;
        }

        private double provideDouble(IContext context) {
            return 0;
        }

        private float provideFloat(IContext context) {
            return 0;
        }
    }
}

