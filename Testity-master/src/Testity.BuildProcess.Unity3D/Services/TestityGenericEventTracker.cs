using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testity.BuildProcess.Unity3D
{
	public class TestityGenericEventTracker
	{
		private readonly object syncObj = new object();

		private readonly Dictionary<string, object> handledGenericClassNames;

		private readonly Dictionary<string, IEnumerable<Type>> eventFreshNameToGenericArgsMap;

		public TestityGenericEventTracker()
		{
			eventFreshNameToGenericArgsMap = new Dictionary<string, IEnumerable<Type>>();
			handledGenericClassNames = new Dictionary<string, object>();
        }

		public void Register(string key, IEnumerable<Type> value)
		{
			//very possible this will be called during a threated build step/task
			lock(syncObj)
			{
				//if we've recently cached it or it has been handled already then we don't need to add it
				if (eventFreshNameToGenericArgsMap.ContainsKey(key) || handledGenericClassNames.ContainsKey(key))
					return;

				Console.WriteLine(key + " been added to tracker.");

				eventFreshNameToGenericArgsMap[key] = value;
			}
		}

		public IEnumerable<KeyValuePair<string, IEnumerable<Type>>> GetAdditionsAndClear()
		{
			lock(syncObj)
			{
				IEnumerable<KeyValuePair<string, IEnumerable<Type>>> temp = eventFreshNameToGenericArgsMap.ToList(); //snap shot the dictionary

				foreach (var kvp in temp)
					handledGenericClassNames.Add(kvp.Key, new object());

				eventFreshNameToGenericArgsMap.Clear(); //clear out the new added values

				return temp;
			}
		}
	}
}
