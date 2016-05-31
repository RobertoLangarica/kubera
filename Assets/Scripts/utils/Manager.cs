using UnityEngine;
using System.Collections;

public abstract class Manager : CachedMonoBehaviour 
{
	public bool	_mustShowDebugInfo;
	public bool	_mustPersistSceneChange;	
	[HideInInspector]
	public bool isThisManagerValid = true;
}

[DisallowMultipleComponent]
public class Manager<T>: Manager where T : Manager
{
	protected static T _cachedInstance;

	private static void SetInstance(T instance)
	{
		if(_cachedInstance == null)
		{
			_cachedInstance = instance;
			if(_cachedInstance != null)
			{
				if(_cachedInstance._mustPersistSceneChange)
				{
					GameObject.DontDestroyOnLoad(_cachedInstance.CachedGameObject);
				}
			}
		}
		else
		{
			if(_cachedInstance._mustShowDebugInfo)
			{
				Debug.LogWarning("Destroying Manager["+instance.name+"] of type["+typeof(T)+"] because there is already one registered by the name["+_cachedInstance.name+"].");
			}
			instance.isThisManagerValid = false;
			Destroy(instance.CachedGameObject);
		}
	}

	public static T GetInstance()
	{
		return _cachedInstance;
	}
		
	public static R GetCastedInstance<R>() where R:T
	{
		return 	((R)_cachedInstance);
	}

	protected virtual void Awake()
	{
		SetInstance(GetComponent<T>());
	}
}


