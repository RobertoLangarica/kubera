using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HighLight : MonoBehaviour 
{
	protected bool isActive;
	protected List<HighLightManager.EHighLightType> suscribedTypes = new List<HighLightManager.EHighLightType> ();
	protected List<HighLightManager.EHighLightStatus> suscribedStatus = new List<HighLightManager.EHighLightStatus> ();

	public bool activateHighLight(HighLightManager.EHighLightType type,HighLightManager.EHighLightStatus status)
	{
		int index = suscribedTypes.IndexOf (type);

		if (index < 0) 
		{
			if (suscribedTypes.Count == 0) 
			{
				gameObject.SetActive (true);
			}

			suscribedTypes.Add (type);
			suscribedStatus.Add (status);

			updateColor ();
			return true;
		}
		return false;
	}

	protected void updateColor()
	{
		Color temp = Color.white;
		Image tempImg = null;
		SpriteRenderer tempSpt = null;

		switch (suscribedStatus[suscribedStatus.Count -1]) 
		{
		case(HighLightManager.EHighLightStatus.NORMAL):
			temp = HighLightManager.GetInstance().normalHighLight;
			break;
		case(HighLightManager.EHighLightStatus.WRONG):
			temp = HighLightManager.GetInstance().wrongHighLight;
			break;
		}

		tempImg = gameObject.GetComponent<Image>();
		if (tempImg != null) 
		{
			tempImg.color = temp;
		} 
		else 
		{
			tempSpt = gameObject.GetComponent<SpriteRenderer> ();
			if (tempSpt != null) 
			{
				tempSpt.color = temp;
			}
		}
	}

	public bool completlyDeactivateType(HighLightManager.EHighLightType type)
	{
		int index = suscribedTypes.IndexOf (type);

		if (index >= 0) 
		{
			suscribedTypes.RemoveAt (index);
			suscribedStatus.RemoveAt (index);

			if (suscribedTypes.Count == 0) 
			{
				gameObject.SetActive (false);
				return true;
			} 
			else 
			{
				updateColor ();
				return false;
			}
		}
		return false;
	}
}