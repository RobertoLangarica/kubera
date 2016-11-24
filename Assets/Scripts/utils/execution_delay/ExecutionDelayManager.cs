using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

public class ExecutionDelayManager : MonoBehaviour 
{
	public int framesToDelay = 5;
	public MonoBehaviour[] scripts;
	private const string AWAKE_METHOD = "DelayedAwake";
	private const string START_METHOD = "DelayedStart";
	private int currentIndex = 0;
	private int scriptsCount;
	private bool awake = true;
	private string methodName;

	void Start()
	{
		scriptsCount = scripts.Length;
		methodName = AWAKE_METHOD;
	}

	void Update()
	{
		if(framesToDelay-- <=0)
		{
			//Ya termino la espera
			if(currentIndex < scriptsCount)
			{
				Type type = scripts[currentIndex].GetType();
				MethodInfo methodInfo = type.GetMethod(methodName,BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

				if(methodInfo != null)
				{
					Debug.Log("Invoking from: "+type.ToString()+"::"+methodName);
					type.InvokeMember(methodName,BindingFlags.InvokeMethod,null,scripts[currentIndex],null);
				}

				currentIndex++;

				if(currentIndex >= scriptsCount)
				{
					//Ya termino de recorrer la lista
					if(awake)
					{
						awake = false;
						methodName = START_METHOD;
						currentIndex = 0;
					}
					else
					{
						//Se desactiva a si mismo
						this.enabled = false;
					}
				}
			}
		}
	}
}
