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

	protected Letter wildCardSelected;
	protected WordManager wordManager;
	protected GameManager gameManager;

	void Start()
	{
		wordManager = FindObjectOfType<WordManager> ();
		gameManager = FindObjectOfType<GameManager> ();

		calculateCellSize ();

		fillContainer (wordManager.wordsValidator.getAlfabet());

		gameObject.SetActive (false);

		powerUP.OnPowerupCompleted += showKeyBoardForWildCard;
	}

	protected void calculateCellSize()
	{
		float width = container.GetComponent<RectTransform> ().rect.size.x;
		Debug.Log (width);

		width = (width - (container.padding.left * 2) - (container.spacing.x * (container.constraintCount - 1))) / container.constraintCount;

		container.cellSize = new Vector2 (width,width);
	}

	protected void fillContainer(List<ABCUnit> alphabet)
	{
		for (int i = 0; i < alphabet.Count; i++) 
		{
			Letter clone = Instantiate(letterPrefab).GetComponent<Letter>();

			ABCChar abcChar = new ABCChar ();

			abcChar.character = alphabet [i].cvalue.ToString();
			abcChar.value = alphabet [i].ivalue;
			clone.abcChar = abcChar;

			clone.updateTexts();

			addLetterToContainer (clone);

			addButton (clone);
		}
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

		btn.onClick.AddListener (()=>{setLetterToWildCard(letter.abcChar.character);});
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
	}

	public void setSelectedWildCard(Letter wildCard)
	{
		wildCardSelected = wildCard;
	}

	protected void setLetterToWildCard(string character)
	{
		wordManager.setValuesToWildCard (wildCardSelected, character);

		hideKeyBoard ();
	}
}
