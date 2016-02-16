using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

	public Text points;
	public Text movementsText;
	public Text gemsText;
	public Text levelText;

	public GameObject GemsChargeGO;

	void Start () {
	
	}

	/**
	 * Setea los puntos en la hud
	 **/
	public void setPoints(int pointsCount)
	{
		points.text = pointsCount.ToString();
	}

	/**
	 * Setea los movimientos en la hud
	 **/
	public void setMovments(int movments)
	{
		movementsText.text = movments.ToString();
	}

	/**
	 * Setea el dinero en la hud
	 **/
	public void setGems (int gems)
	{
		gemsText.text = gems.ToString();
	}

	/**
	 * Setea cuanto se cobrara en la hud
	 **/
	public void setChargeGems(int chargeGems)
	{
		if(GemsChargeGO.transform.FindChild("Charge") != null)
		{
			if (chargeGems == 0) 
			{
				GemsChargeGO.transform.FindChild ("Charge").GetComponentInChildren<Text> ().text = " " + chargeGems.ToString ();
			}
			else
			{
				GemsChargeGO.transform.FindChild ("Charge").GetComponentInChildren<Text> ().text = "-" + chargeGems.ToString ();
			}
		}
	}

	/**
	 * Activa o desactiva el gemsCharge
	 **/
	public void activateChargeGems(bool activate)
	{
		GemsChargeGO.SetActive (activate);
	}

	/**
	 * Setea el nivel en la hud
	 **/
	public void setLevel(int chargeMoney)
	{
		levelText.text = chargeMoney.ToString ();
	}	

	/**
	 * Setea la condicion de victoria
	 **/
	public void setWinCondition(int chargeMoney)
	{

	}	
}
