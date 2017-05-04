using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;
using UnityEngine;

namespace Testity.EngineServices.Unity3D
{
	[EngineServiceConcrete]
	public class UnityEngineLifetimeManagedGameObjectDestructionServiceAdapter : LifetimeManagedGameObjectDestructionServiceAdapter<UnityEngine.GameObject>
	{
		public UnityEngineLifetimeManagedGameObjectDestructionServiceAdapter(ILifetimeManagedEngineObjectRegister<IEngineGameObject, UnityEngine.GameObject> register)
			: base(register)
		{

		}

		protected override void DestroyObject(GameObject toDestroy)
		{
			GameObject.Destroy(toDestroy);
		}

		protected override void DestroyObject(GameObject toDestroy, float time, Action onObjectWasDestroyed)
		{
			throw new NotImplementedException("This is not implemented yet. It's not easy to implement due to registery.");
		}
	}
}
