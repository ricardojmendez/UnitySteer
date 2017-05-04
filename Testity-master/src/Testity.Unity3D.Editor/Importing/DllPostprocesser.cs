using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Testity.Common;
using UnityEditor;
using UnityEngine;

namespace Testity.Unity3D.Editor
{
	public class DllPostprocesser : UnityEditor.AssetPostprocessor
	{
		public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			foreach (string s in importedAssets)
				if (s.Contains(".dll"))
				{
					Debug.Log("Found dll " + s);

					//Load for reflection only. If you actually load the DLL you will have issues
					//if you try to remove it you will be unable to.
					Assembly loadedAss = Assembly.ReflectionOnlyLoadFrom(s);

					//It should load but it might not
					if (loadedAss != null)
					{
						Debug.Log("Successfulled loaded assembly.");

						//Feed information into the testity build process app and
						if (loadedAss.GetCustomAttributes(false).FirstOrDefault(x => x.GetType() == typeof(TestityAssemblyAttribute)) != null)
							//remove Assets from the path and add in the full path.
							//Working directory won't be the same so we can't use relative
							StartTestityProcess(Application.dataPath + s.Trim("Assets".ToCharArray()), "Testity//App//Testity.BuildProcess.Unity3D.exe"); //pass the dll path and start the testity build process
					}
					else
						Debug.Log("Failed to load assembly.");
				}			
		}

		public static void StartTestityProcess(string dllPath, string testityAppPath)
		{
			System.Diagnostics.Process testityProcess = null;

			//see https://msdn.microsoft.com/en-us/library/e8zac0ca(v=vs.110).aspx for error reasons
			try
			{
				testityProcess = System.Diagnostics.Process.Start(testityAppPath, dllPath);
			} 
			catch(Win32Exception e)
			{
				Debug.LogErrorFormat("Unknown error attempting to launch Testity. Could not generate DLL for {0}. Error: {1}", dllPath, e.Message);
				throw;
			}
			catch(FileNotFoundException e)
			{
				Debug.LogErrorFormat("Unable to launch the Testity build application. Error: {0}", e.Message);
				throw;
			}

			//now we must block the thread and wait for the processing to finish.
			//this is probably not a good way to do this Unity doesn't offer a very good extendable pipeline
			while (!testityProcess.HasExited)
			{
				//refresh for up-to-date polling. May not be required
				testityProcess.Refresh();

				testityProcess.WaitForExit(100);

				//this is already a total mess so let's throw in a thread sleep for good measure
				Thread.Sleep(500);
			}

			//at this point the process has completed
			//if it's code 0 then it was a success and we need to reload the asset database
			AssetDatabase.ImportAsset(dllPath.TrimEnd(".dll".ToCharArray()) + ".MonoBehaviours.dll"); //we make the bold assumption that this won't change
		}
	}
}
