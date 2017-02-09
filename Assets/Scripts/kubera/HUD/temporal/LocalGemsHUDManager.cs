using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using utils.gems;

public class LocalGemsHUDManager : MonoBehaviour 
{
	public bool useiOSAsDefault = true;
	public GameObject androidObject;
	public GameObject iOSObject;

	public GameObject allowedGemsObject;
	public GameObject notAllowedGemsObject;

	public Text txtGems;
	public Text txtTime;

	private bool canGrantGems;

	void Start()
	{
		#if UNITY_IOS
		androidObject.SetActive(false);
		iOSObject.SetActive(true);
		#else
		androidObject.SetActive(!useiOSAsDefault);
		iOSObject.SetActive(useiOSAsDefault);
		#endif

		txtGems.text = 	"+" + ShopikaManager.GetCastedInstance<ShopikaManager>().gemsToGiveLocally.ToString("00");

		canGrantGems = ShopikaManager.GetCastedInstance<ShopikaManager>().canGiveLocalGems();
		OnCanGrantGemsChange();
	}

	void Update () 
	{
		//TODO optimizar que esto no sea caad frame
		if(!canGrantGems)
		{
			txtTime.text = ShopikaManager.GetCastedInstance<ShopikaManager>().remainingTimeToGiveGemsString();
		}

		if(canGrantGems != ShopikaManager.GetCastedInstance<ShopikaManager>().canGiveLocalGems())
		{
			canGrantGems = ShopikaManager.GetCastedInstance<ShopikaManager>().canGiveLocalGems();
			OnCanGrantGemsChange();
		}
	}

	void OnCanGrantGemsChange()
	{
		allowedGemsObject.SetActive(canGrantGems);
		notAllowedGemsObject.SetActive(!canGrantGems);
	}
}
