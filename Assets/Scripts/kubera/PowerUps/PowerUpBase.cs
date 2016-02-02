using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerUpBase : MonoBehaviour 
{
	public int uses;

	public GameObject powerUpCursor;
	public Text numberUses;
	public GameObject imageUses;

	public EPOWERUPS typeOfPowerUp;

	protected GameManager gameManager;
	protected CellsManager cellsManager;

	public delegate void rotateActive(bool activate);

	public static rotateActive onRotateActive;

	void Start () 
	{
		gameManager = FindObjectOfType<GameManager>();
		cellsManager = FindObjectOfType<CellsManager>();

		initializeUsesFromUserData();
		numberUses.text = uses.ToString ();
	}

	public void PowerUsed()
	{
		if(uses !=0)
		{
			uses--;
			numberUses.text = uses.ToString ();
			//hacerlo no interactuable
			if(uses==0)
			{
				imageUses.SetActive(false);
				gameObject.GetComponent<Button>().interactable = false;
			}
		}
	}

	public void oneTilePower()
	{
		if (gameManager.canRotate) 
		{
			gameManager.canRotate = false;
			activeDelegateRotation ();
		}

		if (uses != 0) 
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			GameObject go = (GameObject)Instantiate (powerUpCursor);//,new Vector3(pos.x,pos.y+1.5f,1),Quaternion.identity);

			go.GetComponent<Piece>().myFirstPos = powerUpCursor.transform;
			go.GetComponent<Piece>().myFirstPos.position = pos;
			go.name = "PowerOne";
			go.GetComponent<Piece> ().powerUp = true;
			FindObjectOfType<InputGameController> ().activePowerUp (go);
		}
	}

	public void activeRotate(bool activate)
	{
		if (uses != 0) 
		{
			if(activate)
			{
				gameManager.canRotate = true;
			}
			else
			{
				gameManager.canRotate = false;
			}
		}
		else
		{
			gameManager.canRotate = false;
		}

		activeDelegateRotation ();
	}

	protected void activeDelegateRotation()
	{
		if(onRotateActive != null)
		{					
			onRotateActive (gameManager.canRotate);
		}
	}

	/*
	 * Funcion del boton del PowerUp de Destruir por Color
	 */
	public void activateDestroyMode()
	{
		if (gameManager.canRotate) 
		{
			gameManager.canRotate = false;
			activeDelegateRotation ();
		}

		if(uses != 0)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			GameObject go = Instantiate (powerUpCursor) as GameObject;

			go.GetComponent<Piece>().myFirstPos = powerUpCursor.transform;
			go.GetComponent<Piece>().myFirstPos.position = pos;
			go.name = "DestroyPowerUp";
			go.GetComponent<Piece> ().powerUp = true;
			FindObjectOfType<InputGameController> ().activePowerUp (go);

			gameManager.destroyByColor = true;
		}
	}

	public void activateWildCard()
	{
		if (gameManager.canRotate) 
		{
			gameManager.canRotate = false;
			activeDelegateRotation ();
		}
		
		if (uses != 0) 
		{
			GameObject.Find("WordManager").GetComponent<WordManager>().addCharacter(".",gameObject);
			PowerUsed();
			FindObjectOfType<ShowNext>().ShowingNext(true);
		}
	}

	public void returnPower()
	{
		uses++;
		numberUses.text = uses.ToString ();
		imageUses.SetActive(true);
		gameObject.GetComponent<Button>().interactable = true;

	}

	protected void initializeUsesFromUserData()
	{
		switch(name)
		{
		case("PowerRotate"):
		{
			uses = UserDataManager.instance.rotatePowerUpUses;
		}
			break;
		case("PowerOne"):
		{
			uses = UserDataManager.instance.onePiecePowerUpUses;
		}
			break;
		case("WildCard"):
		{
			uses = UserDataManager.instance.wildCardPowerUpUses;
		}
			break;
		case("Destroy"):
		{
			uses = UserDataManager.instance.destroyPowerUpUses;
		}
			break;
		}
	}
}
