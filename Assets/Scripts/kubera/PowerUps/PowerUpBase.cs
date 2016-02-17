using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ABC;

public class PowerUpBase : MonoBehaviour 
{
	public delegate void powerUpUsed();

	public int gemsPrice;

	public GameObject powerUpCursor;

	public EPOWERUPS typeOfPowerUp;

	protected CellsManager cellsManager;

	public powerUpUsed OnPowerUpUsed;

	void Start () 
	{
		cellsManager = FindObjectOfType<CellsManager>();
	}

	public GameObject oneTilePower(Transform myButtonPosition)
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		GameObject go = Instantiate (powerUpCursor,myButtonPosition.position,myButtonPosition.rotation)as GameObject; //,new Vector3(pos.x,pos.y+1.5f,1),Quaternion.identity);
		go.GetComponent<Piece>().myFirstPos = myButtonPosition;
		//go.GetComponent<Piece>().myFirstPos.position = pos;
		go.name = "PowerOne";
		go.GetComponent<Piece> ().powerUp = true;
		return go;
	}

	/*
	 * Funcion del boton del PowerUp de Destruir por Color
	 */
	public GameObject activateDestroyMode(Transform myButtonPosition)
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		GameObject go = Instantiate (powerUpCursor,myButtonPosition.position,myButtonPosition.rotation) as GameObject;

		go.GetComponent<Piece>().myFirstPos = myButtonPosition;
		//go.GetComponent<Piece>().myFirstPos.position = pos;
		go.name = "DestroyPowerUp";
		go.GetComponent<Piece> ().powerUp = true;
		return go;
	}

	public void returnPower()
	{
		gameObject.GetComponent<Button>().interactable = true;
	}

	public void PowerUsed()
	{
		//El cobro de las gemas
	}
}
