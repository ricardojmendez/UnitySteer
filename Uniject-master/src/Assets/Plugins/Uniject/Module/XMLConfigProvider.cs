using System;
using Uniject.Configuration;

namespace Uniject.Impl {

    /// <summary>
    /// Provides XML backed primitive types including string, float and double.
    /// </summary>
    public class XMLConfigProvider<T> {

        private XMLConfigManager manager;

        public XMLConfigProvider(XMLConfigManager manager) {
            this.manager = manager;
        }

        public T CreateInstance(Ninject.Activation.IContext context) {
            XMLConfigValue value = Scoping.getContextAttribute<XMLConfigValue>(context);
            if (value == null) {
                return default(T);
            }
            
            return manager.getValue<T>(value.file, value.xpath);
        }
    }
}

