using System;
using Ninject;
using Ninject.Activation;
using UnityEngine;

public class UnityNinjectSettings : INinjectSettings
{
    T INinjectSettings.Get<T>(string key, T defaultValue) {
        throw new NotImplementedException();
    }

    public Func<IContext, object> DefaultScopeCallback {
        get { return x => new object (); }
    }

	public T Get<T> (string key, T defaultValue)
	{
		throw new NotImplementedException ();
	}

	public void Set (string key, object value)
	{
		throw new NotImplementedException ();
	}

	public Type InjectAttribute {
		get {
			return typeof(Ninject.InjectAttribute);
		}
	}

	public TimeSpan CachePruningInterval {
		get {
			return TimeSpan.FromSeconds(10.0f);
		}
	}

	public bool LoadExtensions {
		get {
			return false;
		}
	}

	public string[] ExtensionSearchPatterns {
		get {
			throw new NotImplementedException ();
		}
	}

	public bool UseReflectionBasedInjection {
		get {
            return Application.platform == RuntimePlatform.IPhonePlayer;
		}
	}

	public bool InjectNonPublic {
		get {
			return false;
		}
		set {
			
		}
	}

	public bool InjectParentPrivateProperties {
		get {
			return false;
		}
		set {
		}
	}

	public bool ActivationCacheDisabled {
		get {
			return true;
		}
		set {
		}
	}

	public bool AllowNullInjection {
		get {
			return false;
		}
		set {
		}
	}
}

