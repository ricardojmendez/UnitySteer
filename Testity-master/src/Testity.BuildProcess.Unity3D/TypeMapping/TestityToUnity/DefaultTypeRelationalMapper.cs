using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testity.EngineComponents;
using Testity.EngineComponents.Unity3D;

namespace Testity.BuildProcess.Unity3D
{
	/// <summary>
	/// Default or last-chance type relational mapper that attempts to map a type to a Unity serializable type.
	/// It can map interfaces. Classes would have to be EngineScriptComponents and those are handled elsewhere
	/// </summary>
	public class DefaultTypeRelationalMapper : ITypeRelationalMapper
	{
		public Type ResolveMappedType(Type typeToFindRelation)
		{
			if (typeToFindRelation == null)
				throw new ArgumentNullException(nameof(typeToFindRelation), "Type cannot be null.");

			//This allows users to serialize interfaces that EngineScriptComponent types may implement
			//Unity editor may not allow users to set TestityBehaviour's that don't contain EngineScriptComponents that implement the interface

			//Exclude types that aren't interfaces
			if (typeToFindRelation.IsInterface)
				return typeof(UnityEngine.MonoBehaviour); //we return a monobehaviour because TestityBehaviours will be assigned to it that have EngineScriptComponents that implement the interface
			else
				return null;
		}
	}
}
