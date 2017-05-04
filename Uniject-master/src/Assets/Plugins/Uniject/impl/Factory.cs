using System;
using Ninject;

namespace Uniject {
    public class Factory<T> {

        private IKernel kernel;
        public Factory(IKernel kernel) {
            this.kernel = kernel;
        }

        public T create() {
            return kernel.Get<T>();
        }
    }
}

