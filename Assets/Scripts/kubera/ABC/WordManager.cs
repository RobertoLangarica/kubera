using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using ABC;
using DG.Tweening;

public class WordManager : MonoBehaviour 
{

	public enum EDeleteState
	{
		WORD,CHARACTER
	}

	public enum EPoolType
	{
		OBSTACLE,NORMAL,TUTORIAL	
	}

	public GameObject gridLetterPrefab;
	public GameObject letterPrefab;
	public GameObject letterContainer;
	[HideInInspector]public Transform gridLettersParent;
	[HideInInspector]public Vector2 gridLettersSizeDelta;


	public GameObject wordCompleteButton;
	public GameObject wordDeleteButton;
	public Image deleteButtonImage;
	public Sprite deleteCharacterState;
	public Sprite deleteWordState;

	private InputWords inputWords;

	[HideInInspector]public ABCDictionary wordsValidator;

	private int maxLetters = 10;
	[HideInInspector]public List<Letter> letters;
	private int siblingIndexAfterDrag;
	private Vector2[] lettersPositions;

	[HideInInspector]public Vector3 deleteBtnPosition;

	protected GridLayoutGroup wordContainerLayout;
	protected RectTransform wordContainerRectTransform;

	[HideInInspector]public int wordPoints;

	private RandomPool<ABCChar> lettersPool;
	private RandomPool<ABCChar> obstaclesLettersPool;
	private RandomPool<ABCChar> tutorialLettersPool;

	public delegate void DOnWordChange(int wordValue);
	public DOnWordChange onWordChange;

	//Letter to take the position for the selectedAnim
	protected Letter lastSelected;
	public float selectAnimationTime = 1;

	void Start()
	{
		letters = new List<Letter>(maxLetters);

		deleteBtnPosition = deleteButtonImage.transform.localPosition;

		//Tamaño de las celdas
		wordContainerLayout = letterContainer.GetComponent<GridLayoutGroup>();
		wordContainerRectTransform = letterContainer.GetComponent<RectTransform> ();

		float cellSize = wordContainerRectTransform.rect.height * 0.9f;

		wordContainerLayout.cellSize = new Vector2(cellSize,cellSize);


		wordsValidator = FindObjectOfType<ABCDictionary>();

		inputWords = FindObjectOfType<InputWords> ();

		if(inputWords)
		{
			inputWords.onTap		+= OnGridLetterTapped;
			inputWords.onTapToDelete+= onLetterTap;
			inputWords.onDragUpdate += OnLetterDragging;
			inputWords.onDragFinish += OnLetterDragFinish;
			inputWords.onDragStart  += OnLetterDragStart;
		}

		activateWordDeleteBtn(false);
		activateWordCompleteBtn(false);
	}

	private void OnGridLetterTapped(GameObject go)
	{
		Letter letter = go.GetComponent<Letter>();

		if(letter.isPreviouslySelected())
		{
			//Se va eliminar
			removeLetter(letter.letterReference);
		}
		else
		{
			//Se va agregar
			addLetterFromGrid(letter);
		}
	}

	private void onLetterTap(GameObject go)
	{
		Letter letter = go.GetComponent<Letter>();
		if(!letter.abcChar.wildcard)
		{
			removeLetter(letter);
		}
	}

	private void OnLetterDragStart(GameObject target)
	{
		activateGridLayout (false);
		fillLettersPositions ();
		siblingIndexAfterDrag = target.transform.GetSiblingIndex();
		setSiblingIndex (target, maxLetters);
		changeDeleteState(EDeleteState.CHARACTER);
	}

	private void fillLettersPositions()
	{
		Transform container = letterContainer.transform;
		lettersPositions = new Vector2[container.childCount];
		for(int i=0; i< container.childCount; i++)
		{
			lettersPositions[i] = container.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
		}
	}

	private void OnLetterDragging(GameObject letter)
	{
		Transform container = letterContainer.transform;
		RectTransform letterRect = letter.GetComponent<RectTransform>();

		for(int i = 0; i< container.childCount; i++)
		{			
			if(container.GetChild(i).gameObject != letter)
			{
				RectTransform childRect = container.GetChild(i).GetComponent<RectTransform>();

				if( 	childRect.anchoredPosition.x > (letterRect.anchoredPosition.x - (letterRect.rect.width*0.2f) ) 
					&&	childRect.anchoredPosition.x < (letterRect.anchoredPosition.x + (letterRect.rect.width*0.2f) ) )
				{
					if(letterRect.anchoredPosition.x > childRect.anchoredPosition.x)
					{
						//izquierda a derecha
						siblingIndexAfterDrag = i;

						for(int j=container.childCount-2; j>=i; j--)
						{
							RectTransform childPosition = container.GetChild (j).GetComponent<RectTransform> ();
							childPosition.anchoredPosition = new Vector2(lettersPositions[j+1].x,childPosition.anchoredPosition.y);
						}
					}
					else
					{
						//derecha a izquierda 
						siblingIndexAfterDrag = i+1;

						for(int j =0; j<=i; j++)
						{
							RectTransform childPosition = container.GetChild (j).GetComponent<RectTransform> ();
							childPosition.anchoredPosition = new Vector2(lettersPositions[j].x,childPosition.anchoredPosition.y);
						}
					}
					break;
				}
			}
		}
	}

	private void OnLetterDragFinish(GameObject target)
	{
		Letter letter = target.GetComponent<Letter>();

		//Los comodines no se pueden destruir
		if(!letter.abcChar.wildcard && isOverDeleteArea(letter.transform.localPosition))
		{
			//removeLetter(letter);
		}
		else
		{
			setSiblingIndex (letter.gameObject, siblingIndexAfterDrag);
		}

		activateGridLayout (true);
		resetValidationToSiblingOrder();
		onLettersChange();
	}

	private bool isOverDeleteArea(Vector3 target)
	{
		if( 	target.x > (deleteBtnPosition.x - (deleteButtonImage.rectTransform.rect.width*0.5f) ) 
			&& 	target.x < (deleteBtnPosition.x + (deleteButtonImage.rectTransform.rect.width*0.5f) )  )
		{
			return true;	
		}

		return false;
	}

	private void setSiblingIndex(GameObject target, int siblingPosition)
	{
		target.transform.SetSiblingIndex (siblingPosition);
	}

	public void addLetterFromGrid(Letter gridReference)
	{
		if(isAddLetterAllowed())
		{
			//Clone para la visualizacion en WordManager
			Letter clone = Instantiate(letterPrefab).GetComponent<Letter>();
			clone.abcChar = gridReference.abcChar;
			clone.type = gridReference.type;
			clone.letterReference = gridReference;
			clone.updateTexts();
			lastSelected = gridReference;

			gridReference.letterReference = clone;

			addLetter(clone);
		}
	}

	private bool isAddLetterAllowed()
	{
		return letters.Count < maxLetters;
	}

	public void addLetter(Letter letter)
	{
		letter.select();
		saveAndValidateLetter(letter);
		selectLetterAnimation(letter);

		onLettersChange();
	}

	protected void saveAndValidateLetter(Letter letter)
	{
		if(letters.Count == 0)
		{
			wordsValidator.initCharByCharValidation();
		}

		letters.Add(letter);
		wordsValidator.validateChar(letter.abcChar);

		afterWordValidation();
	}

	public void removeAllLetters(bool includeWildcards = false)
	{
		int count = letters.Count;
		while(count > 0)
		{
			--count;
			if(!letters[count].abcChar.wildcard)
			{
				removeLetter (letters [count]);
			}
		}

		//Limpiamos la busqueda
		wordsValidator.cleanCharByCharValidation();

		if(!includeWildcards)
		{
			//Reset de la busqueda
			resetValidationToSiblingOrder();
		}

		onLettersChange();
	}

	public void removeLetter(Letter letter)
	{
		letter.deselect();
		GameObject.DestroyImmediate(letter.gameObject);
		letters.Remove(letter);

		resetValidationToSiblingOrder();

		onLettersChange();
		lettersCountChange ();
	}

	private void lettersCountChange ()
	{
		float data;
		float widthGrid = wordContainerRectTransform.rect.width;
		float childCount = letterContainer.transform.childCount; 

		data = wordContainerLayout.cellSize.x * childCount;
		data = widthGrid - data;
		data = (data / childCount - 1);


		if(data < 0)
		{
			wordContainerLayout.spacing = new Vector2( data,0);
		}
		else
		{
			wordContainerLayout.spacing = new Vector2( 0,0);
		}
	}
		
	private void addLetterToContainer(Letter letter)
	{
		//Agregamos la letra al ultimo
		letter.transform.SetParent(letterContainer.transform,false);

		//TODO: Porque se hace este resize
		//para que tengan el collider del tamaño del objeto
		updateLetterBoxCollider (letter.gameObject);

		lettersCountChange ();
	}

	private void selectLetterAnimation(Letter letter)
	{
		letter.transform.SetParent(letterContainer.transform.parent,false);
		letter.transform.position = lastSelected.transform.position;

		GridLayoutGroup grid = letterContainer.GetComponent<GridLayoutGroup> ();
		letter.GetComponent<RectTransform> ().sizeDelta = grid.cellSize;

		Vector3 finalPos = letterContainer.transform.position;

		if (letterContainer.transform.childCount > 0) 
		{
			finalPos = letterContainer.transform.GetChild (letterContainer.transform.childCount - 1).position;
			finalPos.x += grid.cellSize.x * 2 * 0.01f;
		}

		letter.transform.DOMove (finalPos, selectAnimationTime).OnComplete(()=>{addLetterToContainer(letter);});

	}

	private void updateLetterBoxCollider(GameObject letter)
	{
		StartCoroutine(resizeBoxCollider(letter));
	}

	IEnumerator resizeBoxCollider(GameObject letter)
	{
		yield return new WaitForSeconds (0.1f);
		letter.GetComponent<BoxCollider2D> ().size = letter.GetComponent<Image> ().rectTransform.rect.size;
	}

	public void setMaxAllowedLetters(int allowedLetters)
	{
		maxLetters = allowedLetters;
	}

	public bool checkIfAWordIsPossible(List<Letter> pool)
	{
		List<ABCChar> charPool = new List<ABCChar>(pool.Count);

		foreach(Letter l in pool)
		{
			charPool.Add(l.abcChar);
		}

		return checkIfAWordIsPossible(charPool);
	}

	public bool checkIfAWordIsPossible(List<ABCChar> pool)
	{
		if(!wordsValidator.isAWordPossible(pool))
		{
			print("perdio de verdad");
			return false;
		}
		return true;
	}

	private void activateGridLayout(bool activate)
	{
		wordContainerLayout.enabled = activate;
	}

	private void resetValidationToSiblingOrder ()
	{
		letters.Clear();

		for(int i=0; i<letterContainer.transform.childCount; i++)
		{
			letters.Add(letterContainer.transform.GetChild(i).GetComponent<Letter>());
		}

		validateAllLetters();
	}

	private void validateAllLetters()
	{
		wordsValidator.initCharByCharValidation();

		foreach(Letter l in letters)
		{
			wordsValidator.validateChar(l.abcChar);
		}

		//afterWordValidation();
	}

	private void afterWordValidation()
	{
		bool completeWord = wordsValidator.isCompleteWord ();

		activateWordCompleteBtn(completeWord);
		activateWordDeleteBtn (!completeWord,isThereAnyLetterOnContainer());

		if(completeWord)
		{
			Debug.Log("Se completo: "+getCurrentWordOnList());
		}
	}

	public void activateWordCompleteBtn(bool active)
	{
		wordCompleteButton.SetActive (active);
	}

	public void activateWordDeleteBtn(bool activate,bool isThereAnyLetterOnContainer = false)
	{
		if(activate && isThereAnyLetterOnContainer)
		{
			wordDeleteButton.SetActive (true);
		}
		else
		{
			wordDeleteButton.SetActive (false);
		}
	}

	private void onLettersChange()
	{
		updateWordPoints();

		//activateWordDeleteBtn(isThereAnyLetterOnContainer());
		afterWordValidation();
		changeDeleteState(EDeleteState.WORD);

		onWordChange (wordPoints);
	}

	private void updateWordPoints()
	{
		int amount = 0;
		int multiplierHelper = 1;

		for (int i = 0; i < letters.Count; i++) 
		{
			switch (letters[i].abcChar.pointsOrMultiple) 
			{
			case("x2"):
				{
					multiplierHelper *= 2;}
				break;
			case("x3"):
				{
					multiplierHelper *= 3;}
				break;
			case("x4"):
				{
					multiplierHelper *= 4;}
				break;
			case("x5"):
				{
					multiplierHelper *= 5;}
				break;
			default:
				{
					amount += int.Parse (letters[i].abcChar.pointsOrMultiple);}
				break;
			}
		}

		amount *= multiplierHelper;

		wordPoints = amount;
	}

	public string getCurrentWordOnList()
	{
		string result = "";

		foreach(Letter l in letters)
		{
			//result = result+wordsValidator.getStringByValue(l.abcChar.value);
			result = result + l.abcChar.character;
		}

		return result;
	}

	private bool isThereAnyLetterOnContainer()
	{
		return (letterContainer.transform.childCount == 0 ? false:true); 
	}

	public void changeDeleteState(EDeleteState state)
	{
		switch (state) 
		{
		case EDeleteState.WORD:
			deleteButtonImage.sprite = deleteWordState;
			break;
		case EDeleteState.CHARACTER:
			//deleteButtonImage.sprite = deleteCharacterState;
			break;
		}
	}

	public void initializePoolFromCSV(string csv, EPoolType poolType)
	{
		switch(poolType)
		{
		case EPoolType.NORMAL:
			lettersPool = new RandomPool<ABCChar>(getLettersInfoFromCSV(csv));
			break;
		case EPoolType.OBSTACLE:
			obstaclesLettersPool = new RandomPool<ABCChar>(getLettersInfoFromCSV(csv));
			break;
		case EPoolType.TUTORIAL:
			tutorialLettersPool = new RandomPool<ABCChar>(getLettersInfoFromCSV(csv));
			break;
		}
	}

	/**
	 * CSV
	 * cantidad_letra_puntos/multiplo_tipo
	 * ej. 02_A_1_1,10_B_x2_1
	 **/ 
	protected List<ABCChar> getLettersInfoFromCSV(string csv)
	{
		List<ABCChar> result = new List<ABCChar>();
		string[] info = csv.Split(',');
		string[] infoFragments;

		int amount;
		string abc;
		string points_multiplier;
		int type;
		ABCChar abcChar;

		for(int i =0; i<info.Length; i++)
		{
			infoFragments = info[i].Split('_');
			amount = int.Parse(infoFragments[0]);
			abc = infoFragments[1];
			points_multiplier = infoFragments[2];
			type = int.Parse(infoFragments[3]);

			for(int j = 0;j < amount;j++)
			{
				//letter = new ABCCharinfo();
				abcChar = new ABCChar();
				abcChar.character = abc;
				abcChar.pointsOrMultiple = points_multiplier;
				abcChar.typeInfo = type;
				abcChar.value = wordsValidator.getCharValue(abc);

				result.Add(abcChar);
			}
		}

		return result;
	}

	public Letter getWildcard(string pointsOrMultiple)
	{			
		Letter letter = Instantiate(letterPrefab).GetComponent<Letter>();
		string wildcardValue = ".";

		ABCChar abc = new ABCChar();
		abc.value = wordsValidator.getCharValue(wildcardValue);
		abc.wildcard = true;
		abc.character = wildcardValue;
		abc.pointsOrMultiple = pointsOrMultiple;

		letter.type = Letter.EType.NORMAL;
		letter.abcChar = abc;

		letter.updateTexts();

		return letter;
	}

	public Letter getGridLetterFromPool(EPoolType poolType)
	{
		Letter result = null;

		switch(poolType)
		{
		case EPoolType.NORMAL:
			result = createGridLetter(lettersPool.getNextRandomized());
			break;
		case EPoolType.OBSTACLE:
			result = createGridLetter(obstaclesLettersPool.getNextRandomized());
			break;
		case EPoolType.TUTORIAL:
			result = createGridLetter(tutorialLettersPool.getNextRandomized());
			break;
		}

		return result;
	}

	private Letter createGridLetter(ABCChar charInfo)
	{
		Letter letter = getNewEmptyGridLetter();

		letter.initializeFromABCChar(charInfo);
		letter.updateTexts();
		letter.updateColor();

		return letter;
	}

	private Letter getNewEmptyGridLetter()
	{
		GameObject go = Instantiate(gridLetterPrefab)as GameObject;
		RectTransform rectT = go.GetComponent<RectTransform> ();
		BoxCollider2D collider = go.GetComponent<BoxCollider2D>();

		go.transform.SetParent (gridLettersParent,false);
		rectT.sizeDelta = gridLettersSizeDelta;
		collider.enabled = true;
		collider.size =  rectT.rect.size;

		return go.GetComponent<Letter>();
	}
}