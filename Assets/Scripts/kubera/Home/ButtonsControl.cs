using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using utils.gems;
using Kubera.Data;

public class ButtonsControl : MonoBehaviour 
{
	public Image facebookButton;
	public Image facebookLargeButton;
	public Image facebookHighLight;

	public Image shopikaButton;
	public Image shopikaLargeButton;
	public Image shopikaHighLight;

	public FacebookConnectButton fbConnect;
	public KuberaWebView shConnect;

	public float minAlpha = 0.5f;
	public float animStep = 0.02f;

	protected bool facebookAnim;
	protected bool facebookDescending;

	protected bool shopikaAnim;
	protected bool shopikaDescending;

	void Start()
	{
		analizeFacebookStatus ();
		analizeShopikaStatus ();

		fbConnect.OnLoggedIn += changeFacebookButtons;
		fbConnect.OnLoggedOut += analizeFacebookStatus;

		shConnect.OnLoggedIn += changeShopikaButtons;
	}

	public void analizeFacebookStatus()
	{
		if ((DataManagerKubera.GetInstance () as DataManagerKubera).currentUserId == (DataManagerKubera.GetInstance () as DataManagerKubera).ANONYMOUS_USER) 
		{
			facebookLargeButton.gameObject.SetActive (true);
			facebookHighLight.gameObject.SetActive (true);
			facebookButton.gameObject.SetActive (false);
			facebookAnim = true;
		} 
		else 
		{
			facebookButton.gameObject.SetActive (true);
			facebookLargeButton.gameObject.SetActive (false);
			facebookHighLight.gameObject.SetActive (false);
		}
	}

	public void analizeShopikaStatus()
	{
		if (ShopikaManager.GetCastedInstance<ShopikaManager> ().currentUserId == ShopikaManager.GetCastedInstance<ShopikaManager> ().ANONYMOUS_USER) 
		{
			shopikaLargeButton.gameObject.SetActive (true);
			shopikaHighLight.gameObject.SetActive (true);
			shopikaButton.gameObject.SetActive (false);
			shopikaAnim = true;
		}
		else 
		{
			shopikaButton.gameObject.SetActive (true);
			shopikaLargeButton.gameObject.SetActive (false);
			shopikaHighLight.gameObject.SetActive (false);
		}
	}

	public void changeFacebookButtons()
	{
		facebookButton.gameObject.SetActive (true);

		facebookLargeButton.gameObject.SetActive (false);
		facebookHighLight.gameObject.SetActive (false);
		facebookAnim = false;
	}

	public void changeShopikaButtons()
	{
		shopikaButton.gameObject.SetActive (true);

		shopikaLargeButton.gameObject.SetActive (false);
		shopikaHighLight.gameObject.SetActive (false);
		shopikaAnim = false;
	}

	void Update()
	{
		if (facebookAnim) 
		{
			if (facebookDescending) 
			{
				if (getModifiedAlpha(-animStep,facebookHighLight) <= minAlpha) 
				{
					facebookDescending = false;
				}
			} 
			else 
			{

				if (getModifiedAlpha(animStep,facebookHighLight) >= 1) 
				{
					facebookDescending = true;
				}
			}
		}

		if (shopikaAnim) 
		{
			if (shopikaDescending) 
			{
				if (getModifiedAlpha(-animStep,shopikaHighLight) <= minAlpha) 
				{
					shopikaDescending = false;
				}
			} 
			else 
			{

				if (getModifiedAlpha(animStep,shopikaHighLight) >= 1) 
				{
					shopikaDescending = true;
				}
			}
		}
	}

	protected float getModifiedAlpha(float value,Image tempImg,bool reset = false)
	{
		Color tempCol = Color.white;

		if (!reset) 
		{
			tempCol = new Color (tempImg.color.r, tempImg.color.g, tempImg.color.b, tempImg.color.a + value);
		} 

		tempImg.color = tempCol;

		return tempCol.a;
	}
}
