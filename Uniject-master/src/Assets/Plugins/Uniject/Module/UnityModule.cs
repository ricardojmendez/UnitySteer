using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using System;
using Uniject;
using Uniject.Impl;
using Uniject.Configuration;
using UnityEngine;

public class UnityModule : NinjectModule {
    
    public override void Load() {
        Bind<GameObject>().ToProvider<GameObjectProvider>().InScope(Scoping.GameObjectBoundaryScoper);
        Bind<TestableGameObject>().To<UnityGameObject>().InScope(Scoping.GameObjectBoundaryScoper);
        Bind<IAudioSource>().To <UnityAudioSource>();
        Bind<ILogger>().To<UnityLogger>();
        Bind<IRigidBody>().To<UnityRigidBody>().InScope(Scoping.GameObjectBoundaryScoper);
        Bind<INavmeshAgent>().To<UnityNavmeshAgent>().InScope(Scoping.GameObjectBoundaryScoper);
        Bind<ISphereCollider>().To<UnitySphereCollider>().InScope(Scoping.GameObjectBoundaryScoper);
        Bind<IBoxCollider>().To<UnityBoxCollider>().InScope(Scoping.GameObjectBoundaryScoper);
        
        Bind<IMaths>().To<UnityMath>().InSingletonScope();
        Bind<ITime>().To<UnityTime>().InSingletonScope();
        Bind<ILayerMask>().To<UnityLayerMask>().InSingletonScope();
        Bind<IResourceLoader>().To<UnityResourceLoader>().InSingletonScope();
        Bind<IInput>().To<UnityInput>().InSingletonScope();
        Bind<IScreen>().To<UnityScreen>().InSingletonScope();
        Bind<XMLConfigManager>().ToSelf().InSingletonScope();
        Bind<IPhysics>().To<UnityPhysics>().InSingletonScope();
        
        Bind<IAudioListener>().To<UnityAudioListener>();
        Bind<ITransform>().To<UnityTransform>().InScope(Scoping.GameObjectBoundaryScoper);
        Bind<ILight>().To<UnityLight>().InScope(Scoping.GameObjectBoundaryScoper);

        // Resource bindings.
        Bind<TestableGameObject>().ToProvider<PrefabProvider>().WhenTargetHas(typeof(Resource));
        Bind<AudioClip>().ToProvider<ResourceProvider<AudioClip>>().WhenTargetHas(typeof(Resource));
        Bind<PhysicMaterial>().ToProvider<ResourceProvider<PhysicMaterial>>();
        Bind<IPhysicMaterial>().To<UnityPhysicsMaterial>();
    }
}
