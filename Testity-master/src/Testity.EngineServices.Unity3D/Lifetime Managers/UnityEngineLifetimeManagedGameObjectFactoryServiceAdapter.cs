using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;
using Testity.EngineComponents.Unity3D;
using Testity.EngineMath;
using UnityEngine;

namespace Testity.EngineServices.Unity3D
{
	[EngineServiceConcrete]
	public class UnityEngineLifetimeManagedGameObjectFactoryServiceAdapter : LifetimeManagedGameObjectFactoryServiceAdapter<UnityEngine.GameObject>
	{
		public UnityEngineLifetimeManagedGameObjectFactoryServiceAdapter(ILifetimeManagedEngineObjectRegister<IEngineGameObject, UnityEngine.GameObject> register)
			: base(register)
		{

		}

		protected override GameObject CreateActualEngineObject(Vector3<float> position, Quaternion<float> rotation)
		{
			GameObject newObject = new GameObject();

			newObject.transform.position = position.ToUnityVector();
			newObject.transform.rotation = rotation.ToUnityQuat();

			return newObject;
		}

		protected override GameObject CreateActualEngineObject(IEnginePrefab prefab, Vector3<float> position, Quaternion<float> rotation)
		{
			if (prefab == null)
				throw new ArgumentNullException(nameof(prefab), "Prefab service parameter must not be null.");

			//This really isn't a good way to handle things but it's the best way I could come up with.
			UnityEnginePrefabServiceAdapter tryCastedPrefabService = prefab as UnityEnginePrefabServiceAdapter;

			if (tryCastedPrefabService == null)
				throw new ArgumentException("Failed to convert prefab service into Unity3D service. Desired Type: " + nameof(UnityEnginePrefabServiceAdapter) + " Actual Type: " + prefab.GetType(), nameof(prefab));

			return tryCastedPrefabService.Create(position, rotation);
		}

		protected override IEngineGameObject CreateGameObjectAdapter(GameObject actualGameObject)
		{
			if (actualGameObject == null)
				throw new ArgumentNullException(nameof(actualGameObject), nameof(GameObject) + " parameters must not be null.");

			return new UnityEngineGameObjectAdapter(actualGameObject);
		}

		protected override IEnginePrefabedGameObject CreatePrefabbedGameObjectAdapter(IEnginePrefab prefab, GameObject actualGameObject)
		{
			if (actualGameObject == null)
				throw new ArgumentNullException(nameof(actualGameObject), nameof(GameObject) + " parameters must not be null.");

			if (prefab == null)
				throw new ArgumentNullException(nameof(prefab), "Prefab service parameter must not be null.");

			return new UnityEnginePrefabGameObjectAdapter(actualGameObject);
		}
	}
}
