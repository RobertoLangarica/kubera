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
	protected Transform letterContainerTransform;
	[HideInInspector]public Transform gridLettersParent;
	[HideInInspector]public Vector2 gridLettersSizeDelta;


	public GameObject wordCompleteButton;
	public GameObject wordDeleteButton;
	public Image deleteButtonImage;
	public Sprite deleteCharacterState;
	public Sprite deleteWordState;

	public KeyBoardManager keyBoard;

	private InputWords inputWords;

	[HideInInspector]public ABCDictionary wordsValidator;

	private int maxLetters = 10;
	private int l = 0;
	/*[HideInInspector]*/
public List<Letter> letters;
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
	public GameObject gridInvisibleChild;

	public List<GameObject> gridInvisibleChildren;
	public List<GameObject> freeChildren = new List<GameObject>();
	public List<GameObject> occupiedChildren = new List<GameObject>();

	void Awake()
	{
		letters = new List<Letter>(maxLetters);

		deleteBtnPosition = deleteButtonImage.transform.localPosition;

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

	void Start()
	{
		//Tamaño de las celdas
		wordContainerLayout = letterContainer.GetComponent<GridLayoutGroup>();
		wordContainerRectTransform = letterContainer.GetComponent<RectTransform> ();

		float cellSize = wordContainerRectTransform.rect.height * 0.9f;

		wordContainerLayout.cellSize = new Vector2(cellSize,cellSize);

		letterContainerTransform = letterContainer.transform;

		for(int i=0; i<1; i++)
		{
			addInvisibleChildrenToPool ();
		}
	}

	void addInvisibleChildrenToPool()
	{
		GameObject go;
		go = GameObject.Instantiate(gridInvisibleChild);
		freeChildren.Add(go);
		go.transform.SetParent(letterContainerTransform.parent,false);
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
		if (!letter.abcChar.wildcard) 
		{
			removeLetter (letter);
		} 
		else 
		{
			keyBoard.setSelectedWildCard (letter);
			keyBoard.showKeyBoardForWildCard ();
		}
	}

	private void OnLetterDragStart(GameObject target)
	{
		activateGridLayout (false);
		fillLettersPositions ();
		siblingIndexAfterDrag = target.transform.GetSiblingIndex();
		print (siblingIndexAfterDrag);
		setSiblingIndex (target, maxLetters);
		changeDeleteState(EDeleteState.CHARACTER);
	}

	private void fillLettersPositions()
	{
		lettersPositions = new Vector2[letterContainerTransform.childCount];
		for(int i=0; i< letterContainerTransform.childCount; i++)
		{
			lettersPositions[i] = letterContainerTransform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
		}
	}

	private void OnLetterDragging(GameObject letter)
	{
		RectTransform letterRect = letter.GetComponent<RectTransform>();

		for(int i = 0; i< letterContainerTransform.childCount; i++)
		{			
			if(letterContainerTransform.GetChild(i).gameObject != letter)
			{
				RectTransform childRect = letterContainerTransform.GetChild(i).GetComponent<RectTransform>();

				if( 	childRect.anchoredPosition.x > (letterRect.anchoredPosition.x - (letterRect.rect.width*0.2f) ) 
					&&	childRect.anchoredPosition.x < (letterRect.anchoredPosition.x + (letterRect.rect.width*0.2f) ) )
				{
					if(letterRect.anchoredPosition.x > childRect.anchoredPosition.x)
					{
						//izquierda a derecha
						siblingIndexAfterDrag = i;

						for(int j=letterContainerTransform.childCount-2; j>=i; j--)
						{
							RectTransform childPosition = letterContainerTransform.GetChild (j).GetComponent<RectTransform> ();
							childPosition.anchoredPosition = new Vector2(lettersPositions[j+1].x,childPosition.anchoredPosition.y);
						}
					}
					else
					{
						//derecha a izquierda 
						siblingIndexAfterDrag = i+1;

						for(int j =0; j<=i; j++)
						{
							RectTransform childPosition = letterContainerTransform.GetChild (j).GetComponent<RectTransform> ();
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


		setSiblingIndex (letter.gameObject, siblingIndexAfterDrag);


		activateGridLayout (true);
		resetValidationToSiblingOrder();
		onLettersChange();
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

	public bool isAddLetterAllowed()
	{
		return letters.Count < maxLetters;
	}

	public void addLetter(Letter letter,bool withAnimation = true)
	{
		if (letter.abcChar.wildcard) 
		{
			keyBoard.setSelectedWildCard (letter);
		}

		letter.select();
		saveAndValidateLetter(letter);

		if (withAnimation) 
		{
			selectLetterAnimation (letter);
		} 
		else 
		{
			addLetterToContainer(letter);
		}

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
			if(!letters[count].abcChar.wildcard || includeWildcards)
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
		letters.Remove(letter);
		GameObject.DestroyImmediate(letter.gameObject);

		resetValidationToSiblingOrder();

		onLettersChange();
		/*for(int i=0; i<gridInvisibleChildren.Count; i++)
		{
			if(gridInvisibleChildren[i].transform.parent != letterContainer.transform.parent)
			{
				gridInvisibleChildren[i].transform.SetParent(letterContainerTransform.parent);
				break;
			}
			if(i == gridInvisibleChildren.Count-1 )
			{
				print ("omg");
			}
		}*/
	}

	private void lettersCountChange (int readjustingInvisibleChild=0)
	{
		float data;
		float widthGrid = wordContainerRectTransform.rect.width;
		float childCount =letters.Count; 


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
		letter.transform.SetParent(letterContainerTransform,false);

		//Se actualiza el tamaño del collider al tamaño de la letra
		updateLetterBoxCollider (letter.gameObject);


		for(int i=0; i<gridInvisibleChildren.Count; i++)
		{
			if(gridInvisibleChildren[i].transform.parent != letterContainer.transform.parent)
			{
				gridInvisibleChildren[i].transform.SetParent(letterContainerTransform.parent);
				break;
			}
		}
	}

	private void selectLetterAnimation(Letter letter)
	{
		letter.transform.SetParent(letterContainerTransform.parent,false);

		letter.transform.position = lastSelected.transform.position;

		letter.GetComponent<RectTransform> ().sizeDelta = wordContainerLayout.cellSize;

		Vector3 finalPos = letterContainerTransform.position;

		GameObject go = getFreeChild ();
		go.transform.SetParent (letterContainerTransform);

		print (go.transform.position);

		StartCoroutine (doLetterAnimation(letter,go));

	}

	IEnumerator doLetterAnimation(Letter letter,GameObject finalPosObj)
	{
		yield return new WaitForSeconds (0.01f);
		/*if (letterContainerTransform.childCount > 0) 
		{
			finalPos = letterContainerTransform.GetChild (letterContainerTransform.childCount - 1).position;
			finalPos.x += (wordContainerLayout.cellSize.x + wordContainerLayout.spacing.x) * 0.01f;
		}*/
		letter.transform.DOMove ( finalPosObj.transform.position, selectAnimationTime).OnComplete(()=>{addLetterToContainer(letter); releaseChild(finalPosObj);}).SetId(letter.index);

	}

	public GameObject getFreeChild()
	{
		if(freeChildren.Count == 0)
		{
			addInvisibleChildrenToPool();
		}

		GameObject child = freeChildren[0];
		freeChildren.RemoveAt (0);

		occupiedChildren.Add (child);



		return child;
	}

	public void releaseChild(GameObject child)
	{
		for(int i=0; i<occupiedChildren.Count; i++)
		{
			if(occupiedChildren[i] == child)
			{
				child.transform.SetParent(letterContainerTransform.parent);
				occupiedChildren.Remove (child);
			}
		}

		freeChildren.Add (child);
		for(int i=0; i<freeChildren.Count; i++)
		{
			freeChildren [i].transform.position = freeChildren [freeChildren.Count - 1].transform.position;
		}
	}

	private void updateLetterBoxCollider(GameObject letter)
	{
		StartCoroutine(resizeBoxCollider(letter));
	}

	IEnumerator resizeBoxCollider(GameObject letter)
	{
		yield return new WaitForSeconds (0.1f);

		BoxCollider2D letterCollider;
		if (letter != null) 
		{
			letterCollider = letter.GetComponent<BoxCollider2D> ();
			letterCollider.size = letter.GetComponent<Image> ().rectTransform.rect.size;
			letterCollider.enabled = true;
		}
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
			print("no se puede hacer palabra");
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

		for(int i=0; i<letterContainerTransform.childCount; i++)
		{
			letters.Add(letterContainerTransform.GetChild(i).GetComponent<Letter>());
		}

		validateAllLetters();
	}

	private void validateAllLetters()
	{
		wordsValidator.initCharByCharValidation();

		foreach(Letter l in letters)
		{
			if(l != null)
			{
				wordsValidator.validateChar(l.abcChar);
			}
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

	public void activateWordDeleteBtn(bool activate,bool isThereAnyLetterOnContainer = true)
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

		activateWordDeleteBtn(isThereAnyLetterOnContainer());
		afterWordValidation();
		changeDeleteState(EDeleteState.WORD);

		onWordChange (wordPoints);
		lettersCountChange ();
	}

	private void updateWordPoints()
	{
		int amount = 0;
		int multiplierHelper = 1;

		for (int i = 0; i < letters.Count; i++) 
		{
			if(letters[i] == null)
			{
				letters.Remove (letters [i]);
				continue;
			}
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
		return (letters.Count == 0 ? false:true); 
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

	public void setValuesToWildCard(Letter wildCard,string character)
	{
		ABCChar abcChar = new ABCChar ();

		abcChar.wildcard = true;
		abcChar.character = character;
		abcChar.pointsOrMultiple = "x3";

		wildCard.abcChar = abcChar;

		wildCard.updateTexts ();

		resetValidationToSiblingOrder ();
		afterWordValidation ();
		onLettersChange ();
	}
}