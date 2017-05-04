using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.EngineComponents;
using Testity.EngineMath;
using UnityEngine;

namespace Testity.EngineServices.Unity3D
{
	[Serializable]
	[EngineServiceConcrete]
	public class UnityEnginePrefabServiceAdapter : IEnginePrefab
	{
		/// <summary>
		/// Indicates if the Prefab service is available. It may not be if uninitialized by the user.
		/// </summary>
		public bool isAvailable { get { return internalUnityObject != null; } }

		/// <summary>
		/// The name of the Prefab.
		/// </summary>
		public string PrefabName { get; private set; }

		/// <summary>
		/// <see cref="UnityEngine.GameObject"/> stored internally in this adapter.
		/// </summary>
		[SerializeField]
		private readonly UnityEngine.GameObject internalUnityObject;

		public UnityEnginePrefabServiceAdapter(UnityEngine.GameObject unityObject)
		{
			if (unityObject == null)
				throw new ArgumentNullException(nameof(unityObject), "GameObject cannot be null.");

			PrefabName = unityObject.name;

			internalUnityObject = unityObject;
		}

		public GameObject Create(Vector3<float> position, Quaternion<float> rotation)
		{
			return GameObject.Instantiate(internalUnityObject, position.ToUnityVector(), rotation.ToUnityQuat()) as GameObject;
		}

		public GameObject Create(Vector3 position, Quaternion rotation)
		{
			return GameObject.Instantiate(internalUnityObject, position, rotation) as GameObject;
		}
	}
}
