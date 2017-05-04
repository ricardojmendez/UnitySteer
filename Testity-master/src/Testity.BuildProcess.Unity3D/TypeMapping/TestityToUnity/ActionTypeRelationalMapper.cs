using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Testity.Unity3D.Events;
using UnityEngine.Events;

namespace Testity.BuildProcess.Unity3D
{
	public class ActionTypeRelationalMapper : ITypeRelationalMapper
	{
		private readonly IEnumerable<Type> validGenericActionTypes = new Type[] { typeof(Action<>), typeof(Action<,>), typeof(Action<,,>), typeof(Action<,,,>) };

		public Type ResolveMappedType(Type typeToFindRelation)
		{
			//Check to see if it's a handable action type
			if (!isActionType(typeToFindRelation))
				return null;

			//if(typeToFindRelation.IsGenericType)
			return FindValidUnityEventType(typeToFindRelation);
		}

		public bool isActionType(Type t)
		{
			//It must either be Action or Action<...>
			return typeof(Action) == t || (t.IsGenericType && validGenericActionTypes.Contains((t.GetGenericTypeDefinition())));
        }

		public bool isGenericActionType(Type t)
		{
			return isActionType(t) && t.IsGenericType;
		}

		public string GetSharedGenericEventTypeName(IEnumerable<Type> genericTypeArgs)
		{
			if (genericTypeArgs == null)
				throw new ArgumentNullException(nameof(genericTypeArgs), "Cannot be a null collection of args.");

			//Regex: http://stackoverflow.com/questions/3210393/how-do-i-remove-all-non-alphanumeric-characters-from-a-string-except-dash
			//Removes non-alpha numeric characters to make a valid TypeName
			return Regex.Replace(nameof(TestityEvent) + genericTypeArgs.Aggregate("", (x, y) => x + y.FullName), @"[^a-zA-Z0-9 -]", string.Empty); 
		}

		private Type FindValidUnityEventType(Type actionType)
		{
			if (actionType == typeof(Action))
				return typeof(TestityEvent);

			//should be a generic type at this point
			if (actionType.IsGenericType)
			{
				//grabs the <..> inner type args from the type
				Type[] genericArgs = actionType.GetGenericArguments();

				Type unityEventType = FindUnityEventTypeByArgCount(genericArgs.Count());

				//Now we build a generic UnityEvent Type with the args and generic type.
				return unityEventType.MakeGenericType(genericArgs);
			}
			else
				return null;
		}

		public Type FindUnityEventTypeByArgCount(int argCount)
		{
			switch(argCount)
			{
				//Shouldn't need to do this
				case 0:
					return typeof(TestityEvent);
				case 1:
					return typeof(TestityEvent<>);
				case 2:
					return typeof(TestityEvent<,>);
				case 3:
					return typeof(TestityEvent<,,>);
				case 4:
					return typeof(TestityEvent<,,,>);

				default:
					throw new ArgumentException("Cannot generate " + nameof(TestityEvent) + " generic type with " + argCount + " limit is 0-4 type args.");
			}
		}
	}
}
