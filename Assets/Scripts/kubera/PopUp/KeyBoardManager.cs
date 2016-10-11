using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ABC;

public class KeyBoardManager : MonoBehaviour 
{
	public GameObject letterPrefab;
	public GridLayoutGroup container;

	public WildCardPowerUp powerUP;

	public delegate void DLetterPressed(string str);
	public DLetterPressed OnLetterSelected;

	protected Letter wildCardSelected;
	public WordManager wordManager;
	public GameManager gameManager;
	protected List<RectTransform> keys = new List<RectTransform>();

	void Start()
	{
		calculateCellSize ();

		powerUP.OnPowerupCompleted += showKeyBoardForWildCard;

		OnLetterSelected += setLetterToWildCard;

		if (PersistentData.GetInstance().abcDictionary.getAlfabet () == null) 
		{
			PersistentData.GetInstance().onDictionaryFinished += dictionaryReadyToread;
		} 
		else 
		{
			dictionaryReadyToread ();
		}
	}

	protected void calculateCellSize()
	{
		float width = container.GetComponent<RectTransform> ().rect.size.x;

		width = (width - (container.padding.left * 2) - (container.spacing.x * (container.constraintCount - 1))) / container.constraintCount;

		container.cellSize = new Vector2 (width,width);
	}

	public void dictionaryReadyToread()
	{
		fillContainer (PersistentData.GetInstance().abcDictionary.getAlfabet ());
	}

	protected void fillContainer(List<ABCUnit> alphabet)
	{
		for (int i = 0; i < alphabet.Count; i++) 
		{
			Letter clone = Instantiate(letterPrefab).GetComponent<Letter>();
			keys.Add (clone.GetComponent<RectTransform>());

			ABCChar abcChar = new ABCChar ();

			abcChar.character = alphabet [i].cvalue.ToString();
			abcChar.value = alphabet [i].ivalue;
			clone.abcChar = abcChar;

			clone.updateTexts();

			addLetterToContainer (clone);

			addButton (clone);
		}

		Invoke("removeGridContainerAndMoveAnchors",0.1f);
	}

	protected void addLetterToContainer(Letter letter)
	{
		//Agregamos la letra al ultimo
		letter.transform.SetParent(container.transform,false);

		letter.gameObject.layer = container.gameObject.layer;

		//Se actualiza el tamaño del collider al tamaño de la letra
		BoxCollider2D letterCollider;
		letterCollider = letter.GetComponent<BoxCollider2D> ();
		letterCollider.size = container.cellSize;
		letterCollider.enabled = false;
	}

	protected void addButton(Letter letter)
	{
		Button btn = letter.gameObject.AddComponent<Button> ();

		btn.onClick.AddListener (()=>{
			if (OnLetterSelected != null) 
			{
				OnLetterSelected (letter.abcChar.character);
			}

			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("letterKeyboardChoosed");
				AudioManager.GetInstance().Play("letterKeyboardChoosed");
			}
		});
	}

	protected IEnumerator removeGridContainerAndMoveAnchors()
	{
		yield return new WaitForEndOfFrame();
		container.enabled = false;

		yield return new WaitForEndOfFrame();
		keys.Adjust ();
	}

	public void hideKeyBoard()
	{
		gameObject.SetActive (false);
		gameManager.popUpCompleted ();
	}

	public void showKeyBoardForWildCard()
	{
		gameObject.SetActive (true);
		gameManager.allowGameInput (false);
		moveAnchorsTo0 ();

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("showKeyBoard");
			AudioManager.GetInstance().Play("showKeyBoard");
		}
	}

	public void setSelectedWildCard(Letter wildCard)
	{
		wildCardSelected = wildCard;
	}

	public Letter getSelectedWildCard()
	{
		return wildCardSelected;
	}

	public void setLetterToWildCard(string character)
	{
		if(wildCardSelected.abcChar.character != "A" && wildCardSelected.abcChar.character != character)
		{
			gameManager.useHintWord ();
		}
		wordManager.setValuesToWildCard (wildCardSelected, character);
		hideKeyBoard ();

		wordManager.addLetterFromGrid (wildCardSelected);
		gameManager.updatePiecesLightAndUpdateLetterState ();
	}

	protected void moveAnchorsTo0()
	{
		RectTransform rectT = GetComponent<RectTransform> ();

		rectT.anchorMax = new Vector2 (rectT.anchorMax.x,1);
		rectT.anchorMin = new Vector2 (rectT.anchorMin.x,0);

		//Debug.Log ("Debi moverme a 0");
	}
}
