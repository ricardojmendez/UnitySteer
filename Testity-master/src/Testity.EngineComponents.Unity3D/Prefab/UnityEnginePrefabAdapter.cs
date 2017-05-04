using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;

namespace Testity.EngineComponents
{
	[EngineComponentConcrete(typeof(IEnginePrefab))]
	[EngineComponentAdapter(typeof(UnityEngine.GameObject))]
	public class UnityEnginePrefabAdapter : IEnginePrefab
	{
		//Should be available as long as it's not null
		public bool isAvailable { get { return prefabbedGameObjectInstance != null; } }

		public string PrefabName { get; private set; }

		private readonly UnityEngine.GameObject prefabbedGameObjectInstance;

		[EngineComponentAdapterConstructor(typeof(UnityEngine.GameObject))]
		public UnityEnginePrefabAdapter(UnityEngine.GameObject gameObjectPrefab)
		{
			if (gameObjectPrefab == null)
				throw new ArgumentNullException(nameof(gameObjectPrefab), nameof(UnityEngine.GameObject) + " must not be null.");

			//Could change so we should grab it now
			PrefabName = prefabbedGameObjectInstance.name;

			prefabbedGameObjectInstance = gameObjectPrefab;
		}
	}
}
