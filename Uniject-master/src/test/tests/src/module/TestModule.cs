using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Tests;
using Uniject;
using Moq;

namespace Tests {
    class TestModule : Ninject.Modules.NinjectModule {

        public TestModule() {
        }

        public override void Load () {
            Rebind<ILayerMask>().To<MockLayerMask>();
            Rebind<ITime>().To<MockTime>().InSingletonScope();
            Rebind<ILogger>().To<TestLogger>();
            Rebind<IAudioListener>().To<FakeAudioListener>();
            Rebind<INavmeshAgent>().To<FakeNavmeshAgent>().InScope(Scoping.GameObjectBoundaryScoper);

            Rebind<IRigidBody>().ToProvider<MockProvider<IRigidBody>>().InScope(Scoping.GameObjectBoundaryScoper);
            Rebind<ISphereCollider>().ToProvider<MockProvider<ISphereCollider>>().InScope(Scoping.GameObjectBoundaryScoper);
            Rebind<IBoxCollider>().ToProvider<MockProvider<IBoxCollider>>().InScope(Scoping.GameObjectBoundaryScoper);
            Rebind<ILight>().ToProvider<MockProvider<ILight>>().InScope(Scoping.GameObjectBoundaryScoper);

            Rebind<IAudioSource>().To<FakeAudioSource>().InScope(Scoping.GameObjectBoundaryScoper);
            Rebind<IUtil>().To<MockUtil>().InSingletonScope();
            Rebind<IResourceLoader>().To<MockResourceLoader>().InSingletonScope();
            Rebind<TestableGameObject>().To<FakeGameObject>().InScope(Scoping.GameObjectBoundaryScoper);
            Rebind<ITransform>().To<FakeGameObject.FakeTransform>().InScope(Scoping.GameObjectBoundaryScoper);

            Bind<TestUpdatableManager>().ToSelf().InSingletonScope();

            Bind<TestableGameObject>().ToProvider<PrefabProvider>().WhenTargetHas(typeof(Resource));
            Rebind<IPhysicMaterial>().ToProvider<MockProvider<IPhysicMaterial>>();
            Rebind<IPhysics>().ToProvider<MockProvider<IPhysics>>().InSingletonScope();
            Rebind<IInput>().ToProvider<MockProvider<IInput>>().InSingletonScope();
            Rebind<IScreen>().ToProvider<MockProvider<IScreen>>().InSingletonScope();
        }
    }
}
