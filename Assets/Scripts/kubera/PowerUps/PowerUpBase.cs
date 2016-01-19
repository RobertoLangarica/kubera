using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerUpBase : MonoBehaviour {

	public int uses;

	public GameObject powerOne;
	public Text numberUses;
	public GameObject imageUses;

	protected GameManager gameManager;
	protected CellsManager cellsManager;

	void Start () 
	{
		gameManager = FindObjectOfType<GameManager>();
		cellsManager = FindObjectOfType<CellsManager>();
		numberUses.text = uses.ToString ();
	}

	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			activateDestroyMode();
		}
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
		if (uses != 0) 
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			GameObject go = (GameObject)Instantiate (powerOne);//,new Vector3(pos.x,pos.y+1.5f,1),Quaternion.identity);

			go.GetComponent<Piece>().myFirstPos = powerOne.transform;
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
	}

	/*
	 * Funcion del boton del PowerUp de Destruir por Color
	 */
	public void activateDestroyMode()
	{
		gameManager.destroyByColor = true;

		cellsManager.activatePositionedPiecesCollider();
	}

	public void activateWildCard()
	{
		if (uses != 0) 
		{
			GameObject.Find("WordManager").GetComponent<WordManager>().addCharacter(".",gameObject);
			PowerUsed();
		}
	}

	public void returnPower()
	{
		uses++;
		numberUses.text = uses.ToString ();
		imageUses.SetActive(true);
		gameObject.GetComponent<Button>().interactable = true;
		FindObjectOfType<ShowNext>().ShowingNext(true);
	}
}
