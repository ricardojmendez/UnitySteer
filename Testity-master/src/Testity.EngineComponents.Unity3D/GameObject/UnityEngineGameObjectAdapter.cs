using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Testity.Common;
using Testity.Common.Unity3D;
using UnityEngine;

namespace Testity.EngineComponents.Unity3D
{
	/// <summary>
	/// Unity3D adapter for <see cref="UnityEngine.GameObject"/> that maps it to the Testity <see cref="IEngineGameObject"/> interface.
	/// </summary>
	[EngineComponentConcrete(typeof(IEngineGameObject))]
	[EngineComponentAdapter(typeof(UnityEngine.GameObject))]
	public class UnityEngineGameObjectAdapter : IEngineGameObject
	{
		private readonly UnityEngine.GameObject unityGameObjectAdaptee;
		
		//we don't initialize this immediately because consumers of the GameObject may not need the transform.
		private readonly Lazy<UnityEngineTransformAdapter> cachedTransform;

		private readonly object syncObj = new object();

		public bool ActiveAbsolute
		{
			//Use active in hierarchy because this includes all parent's.
			get { return unityGameObjectAdaptee.activeInHierarchy; }
		}

		public bool ActiveSelf
		{
			get { return unityGameObjectAdaptee.activeSelf; } //Use self because it ignores parent or child active state.
			set { unityGameObjectAdaptee.SetActive(value); } //We can't use property setter so we must make this function call.
		}

		public IEngineDirectional Directions
		{
			get { return cachedTransform.Value; }
		}

		public IEngineTransform Transform
		{
			get { return cachedTransform.Value; }
		}

		public virtual string Name
		{
			get { return unityGameObjectAdaptee.name; }
			set { unityGameObjectAdaptee.name = value; }
		}

		[EngineComponentAdapterConstructor(typeof(UnityEngine.GameObject))]
		public UnityEngineGameObjectAdapter(UnityEngine.GameObject unityGO)
		{
			if (unityGO == null)
				throw new ArgumentNullException(nameof(unityGO), nameof(UnityEngine.GameObject) + " arguement must not be null at construction.");

			unityGameObjectAdaptee = unityGO;

			cachedTransform = new Lazy<UnityEngineTransformAdapter>(() => new UnityEngineTransformAdapter(unityGameObjectAdaptee.transform), true);
		}

		public void Dispose()
		{
			//UnityEngine objects don't need to do anything after disposal.
		}

		public bool Equals(IEngineGameObject other)
		{
			UnityEngineGameObjectAdapter otherGO = null;

			//can't is/as down cast interface.
			if (other.GetType() == typeof(UnityEngineGameObjectAdapter))
				otherGO = Convert.ChangeType(other, typeof(UnityEngineGameObjectAdapter)) as UnityEngineGameObjectAdapter;

			return otherGO == null ? false : (otherGO.unityGameObjectAdaptee == unityGameObjectAdaptee);
		}

		public bool Equals(IEngineObject other)
		{
			UnityEngineGameObjectAdapter otherGO = null;

			//can't is/as down cast interface.
			if (other.GetType() == typeof(UnityEngineGameObjectAdapter))
				otherGO = Convert.ChangeType(other, typeof(UnityEngineGameObjectAdapter)) as UnityEngineGameObjectAdapter;

			return otherGO == null ? false : (otherGO.unityGameObjectAdaptee == unityGameObjectAdaptee);
		}
	}
}
