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

	public enum EWordState
	{
		NO_WORDS_AVAILABLE,WORDS_AVAILABLE,HINTED_WORDS
	}

	public enum EPoolType
	{
		OBSTACLE,NORMAL,TUTORIAL	
	}

	public GameObject gridLetterPrefab;
	public GameObject letterPrefab;
	public GameObject letterContainer;
	protected Transform letterContainerTransform;
	public Transform preLetterContainerTransform;
	public Transform LetterAnimatedContainerTransform;
	[HideInInspector]public Transform gridLettersParent;
	[HideInInspector]public Vector2 gridLettersSizeDelta;


	public GameObject wordCompleteButton;
	public GameObject wordDeleteButton;
	public Image deleteButtonImage;


	public KeyBoardManager keyBoard;
	public InputWords inputWords;

	[HideInInspector]public ABCDictionary wordsValidator;

	private int maxLetters = 10;
	/*[HideInInspector]*/
	public List<Letter> letters;
	protected List<Letter> lettersRemoval  = new List<Letter>();
	public int siblingIndexAfterDrag;
	public Vector2[] lettersPositions;

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
	[HideInInspector]public bool cancelHint = true;

	public float selectAnimationTime = 1;
	public GameObject gridInvisibleChild;

	public List<GameObject> freeChildren = new List<GameObject>();
	public List<GameObject> occupiedChildren = new List<GameObject>();
	[HideInInspector]public GameObject goByDrag;

	public GameObject noWordPosible;
	public Text noWordPosibleText;
	public GameObject points;

	public EWordState currentWordPosibleState;

	protected float centerVacuum;
	protected RectTransform wordCompleteButtonRectTransform;
	protected RectTransform wordDeleteButtonRectTransform;
	void Awake()
	{
		letters = new List<Letter>(maxLetters);
		currentWordPosibleState = EWordState.WORDS_AVAILABLE;

		if (PersistentData.GetInstance ()) 
		{
			
			wordsValidator = PersistentData.GetInstance ().abcDictionary;
		}
		else
		{
			wordsValidator = FindObjectOfType<ABCDictionary> ();
		}
		if(inputWords)
		{
			inputWords.onTap		+= OnGridLetterTapped;
			inputWords.onTapToDelete+= onLetterTap;
			inputWords.onLetterOnGridDragFinish+= OnLetterDragStart;

			inputWords.onDragUpdate += OnLetterDragging;
			inputWords.onDragFinish += OnLetterDragFinish;
			inputWords.onDragStart  += OnLetterDragStart;

			inputWords.onChangePutLetterOverContainer += onChangePutLetterOverContainer;
		}

		wordCompleteButtonRectTransform = wordCompleteButton.GetComponent<RectTransform>();
		wordDeleteButtonRectTransform = wordDeleteButton.GetComponent<RectTransform>();

		//wordCompleteButtonRectTransform.anchoredPosition = new Vector2 (Screen.width * 0.5f, wordCompleteButtonRectTransform.anchoredPosition.y);
		//wordDeleteButtonRectTransform.anchoredPosition = new Vector2 (Screen.width * 0.5f, wordDeleteButtonRectTransform.anchoredPosition.y);


		activateWordBtn(false,false);
		activatePointsGO(false);
		activateNoWordPosibleText (false);

		centerVacuum = Screen.width * 0.5f;

		}

	void Start()
	{
		//Tamaño de las celdas
		wordContainerLayout = letterContainer.GetComponent<GridLayoutGroup>();
		wordContainerRectTransform = letterContainer.GetComponent<RectTransform> ();
		noWordPosible.transform.position = letterContainer.transform.position;

		float cellSize = wordContainerRectTransform.rect.height * 0.9f;

		wordContainerLayout.cellSize = new Vector2(cellSize,cellSize);

		letterContainerTransform = letterContainer.transform;

		for(int i=0; i<5; i++)
		{
			addInvisibleChildrenToPool ();
		}

		inputWords.limitWidth = wordContainerRectTransform.rect.width;
		print (wordContainerRectTransform.rect.width);

	}

	void addInvisibleChildrenToPool()
	{
		GameObject go;
		go = GameObject.Instantiate(gridInvisibleChild);
		go.name = "stock";
		freeChildren.Add(go);
		go.transform.SetParent(letterContainerTransform.parent,false);
	}

	public void OnGridLetterTapped(GameObject go,bool byDrag)
	{
		Letter letter = go.GetComponent<Letter>();

		if(letter.isPreviouslySelected())
		{
			//Se va eliminar
			removeLetter(letter.letterReference);
			arrangeSortingOrder ();
			StartCoroutine( correctTweens ());
		}
		else
		{
			//Se va agregar
			if (isAddLetterAllowed ()) 
			{
				if(!byDrag)
				{
					StartCoroutine( correctTweens ());
					addLetterFromGrid (letter);		
				}
				else
				{
					addLetterFromGrid (letter,byDrag);
				}
			}
		}
	}

	IEnumerator correctTweens (float delay = 0)
	{
		yield return new WaitForSeconds (delay);
		List<Tween> tweens = new List<Tween> ();

		int index = 0;
		for(int i=0; i<letters.Count; i++)
		{			
			tweens = DOTween.TweensById (letters[i].GetInstanceID());

			if(tweens != null)
			{
				//Transform a = (Transform)tweens [0].target;
				//a.DOMove (occupiedChildren [index].transform.position, selectAnimationTime).OnComplete(()=>{addLetterToContainer(letters [i]); releaseChild(occupiedChildren [index]); reacomodateChildrenSiblingOrder();}).SetId(letters[i].GetInstanceID());

				DOTween.Kill (letters [i].GetInstanceID ());
				Letter letter = letters [i];

				GameObject go = occupiedChildren [index];

				fixMoveAnimation (letter, go);
				//StartCoroutine (callback (selectAnimationTime,i,index));

				index++;
			}
		}
	}

	private void fixMoveAnimation(Letter letter,GameObject go)
	{
		letter.transform.DOMove (go.transform.position, selectAnimationTime).OnComplete (() => {callback (letter,go);}).SetId (letter.GetInstanceID ());
	}

	private void callback(Letter letter,GameObject go)
	{
		addLetterToContainer (letter);
		releaseChild (go);
		reacomodateChildrenSiblingOrder ();
	}

	public void onLetterTap(GameObject go,bool byDrag)
	{
		Letter letter = go.GetComponent<Letter>();


		if (!letter.abcChar.wildcard && !letter.wildCard) 
		{
			removeLetter (letter);
			arrangeSortingOrder ();
		} 
		else 
		{
			keyBoard.setSelectedWildCard (letter);
			keyBoard.showKeyBoardForWildCard ();
		}
	}

	private void OnLetterDragStart(GameObject target)
	{
		fillLettersPositions ();
		siblingIndexAfterDrag = target.transform.GetSiblingIndex();
		//setSiblingIndex (target, maxLetters);
		changeDeleteState(EDeleteState.CHARACTER);
		target.GetComponent<Canvas> ().sortingOrder = maxLetters + 1;
	}

	private void onChangePutLetterOverContainer(GameObject go,bool correctlyOnContainer)
	{
		if(correctlyOnContainer)
		{
			go.transform.SetParent(letterContainerTransform,false);
			fillLettersPositions ();
			if(goByDrag)
			{
				releaseChild (goByDrag);
			}
			activateGridLayout (false);
			updateLetterBoxCollider (go);
			onContainerSetSiblingIndex (go);
		}
		else
		{
			go.transform.SetParent(preLetterContainerTransform,false);
			goByDrag = getFreeChild ();
			goByDrag.transform.SetParent (letterContainerTransform);
			goByDrag.transform.SetSiblingIndex (letters.Count);
			fillLettersPositions ();
			activateGridLayout (true);
		}
	}

	private void fillLettersPositions()
	{
		lettersPositions = new Vector2[letterContainerTransform.childCount];
		for(int i=0; i< letterContainerTransform.childCount; i++)
		{
			lettersPositions[i] = letterContainerTransform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
		}
	}

	private void OnLetterDragging(GameObject letter,bool onContainer)
	{
		RectTransform letterRect = letter.GetComponent<RectTransform>();
		for(int i = 0; i< letterContainerTransform.childCount; i++)
		{			
			if(onContainer && letterContainerTransform.GetChild(i).gameObject != letter)
			{
				RectTransform childRect = letterContainerTransform.GetChild(i).GetComponent<RectTransform>();

				if( 	childRect.anchoredPosition.x > (letterRect.anchoredPosition.x - (letterRect.rect.width*0.3f) ) 
					&&	childRect.anchoredPosition.x < (letterRect.anchoredPosition.x + (letterRect.rect.width*0.3f) ) )
				{
					siblingIndexAfterDrag = i;
					setSiblingIndex (letter, siblingIndexAfterDrag);
					setPositionToCurrentDraggingLetter (letter);
					break;
				}
			}
		}
	}

	protected void setPositionToCurrentDraggingLetter(GameObject letter)
	{
		Transform letterContainerChild;
		for (int i = 0; i < letterContainerTransform.childCount; i++) 
		{
			letterContainerChild = letterContainerTransform.transform.GetChild (i);
			if(letter.transform != letterContainerChild )
			{
				RectTransform childPosition = letterContainerTransform.GetChild (i).GetComponent<RectTransform> ();
				childPosition.anchoredPosition = new Vector2(lettersPositions[i].x,childPosition.anchoredPosition.y);
				if(letterContainerChild.name != "stock")
				{
					letterContainerTransform.GetChild (i).GetComponent<Canvas> ().sortingOrder = i;
				}
			}
		}
	}

	protected void onContainerSetSiblingIndex(GameObject letter)
	{
		RectTransform first = letterContainerTransform.GetChild (0).GetComponent<RectTransform> ();
		RectTransform last = letterContainerTransform.GetChild (letterContainerTransform.childCount - 1).GetComponent<RectTransform> ();
		RectTransform letterRect = letter.GetComponent<RectTransform> ();
	
		if(letterRect.anchoredPosition.x < first.anchoredPosition.x)
		{
			siblingIndexAfterDrag = 0;
			setSiblingIndex (letter, siblingIndexAfterDrag);
			setPositionToCurrentDraggingLetter (letter);
		}
		else if(letterRect.anchoredPosition.x == last.anchoredPosition.x)
		{
			siblingIndexAfterDrag = letterContainerTransform.childCount - 1;
			setSiblingIndex (letter, siblingIndexAfterDrag);
			setPositionToCurrentDraggingLetter (letter);
		}
	}

	private void OnLetterDragFinish(GameObject target,bool releaseInContainer)
	{
		Letter letter = target.GetComponent<Letter>();

		setSiblingIndex (letter.gameObject, siblingIndexAfterDrag);

		activateGridLayout (true);
		resetValidationToSiblingOrder();
		onLettersChange();
		target.GetComponent<Canvas> ().sortingOrder = siblingIndexAfterDrag;

		if(!releaseInContainer && goByDrag)
		{
			releaseChild (goByDrag);
			goByDrag = null;
		}
	}

	private void setSiblingIndex(GameObject target, int siblingPosition)
	{
		target.transform.SetSiblingIndex (siblingPosition);
	}

	public void addLetterFromGrid(Letter gridReference,bool byDrag = false)
	{
		//Clone para la visualizacion en WordManager
		Letter clone = Instantiate(letterPrefab).GetComponent<Letter>();
		clone.abcChar = gridReference.abcChar;
		clone.type = gridReference.type;
		clone.letterReference = gridReference;
		clone.updateTexts();
		lastSelected = gridReference;

		gridReference.letterReference = clone;
		//clone.transform.SetParent (preLetterContainerTransform,false);

		if(byDrag)
		{
			addLetter(clone,false);
			inputWords.letter = clone.gameObject;
		}
		else
		{
			addLetter(clone);
			clone.GetComponent<Canvas> ().sortingOrder = letters.Count - 1;
		}
	}

	public bool isAddLetterAllowed()
	{
		return letters.Count < maxLetters;
	}

	public void addLetter(Letter letter,bool withAnimation = true,bool wildCard = false)
	{
		if (letter.wildCard) 
		{
			keyBoard.setSelectedWildCard (letter);
		}

		letter.select();
		saveAndValidateLetter(letter);

		onLettersChange();
		if (withAnimation) 
		{
			selectLetterAnimation (letter);
		}
		else if(wildCard)
		{			
			addLetterToContainer(letter);
		}
		else 
		{
			letter.transform.SetParent(preLetterContainerTransform,false);
			letter.transform.position = lastSelected.transform.position;
			letter.GetComponent<RectTransform> ().sizeDelta = wordContainerLayout.cellSize;

			//addLetterToContainer(letter);
		}

	}

	protected void saveAndValidateLetter(Letter letter)
	{
		if(letters.Count == 0)
		{
			wordsValidator.initCharByCharValidation();
		}

		letters.Add(letter);
		wordsValidator.validateChar(letter.abcChar);

		//afterWordValidation();
	}

	public void removeAllLetters(bool includeWildcards = false)
	{
		int count = letters.Count;
		while(count > 0)
		{
			--count;
			if(!letters[count].wildCard || includeWildcards)
			{
				removeLetter (letters [count]);
			}
		}
		arrangeSortingOrder ();

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
		//letters.Remove(letter);
		if (DOTween.IsTweening (letter.GetInstanceID()))
		{
			DOTween.Complete (letter.GetInstanceID());
		}
		GameObject.DestroyImmediate(letter.gameObject);

		resetValidationToSiblingOrder();

		onLettersChange();
	}

	public void arrangeSortingOrder()
	{
		for(int i =0; i<letters.Count; i++)
		{
			letters [i].GetComponent<Canvas> ().sortingOrder = i;
		}
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
		if(letter.transform.parent != LetterAnimatedContainerTransform)
		{
			//Agregamos la letra al ultimo
			letter.transform.SetParent(letterContainerTransform,false);

			//Se actualiza el tamaño del collider al tamaño de la letra
			updateLetterBoxCollider (letter.gameObject);
		}
	}

	private void selectLetterAnimation(Letter letter)
	{
		letter.transform.SetParent(preLetterContainerTransform,false);

		letter.transform.position = lastSelected.transform.position;

		letter.GetComponent<RectTransform> ().sizeDelta = wordContainerLayout.cellSize;

		GameObject go = getFreeChild ();
		go.transform.SetParent (letterContainerTransform);

		StartCoroutine (doLetterAnimation(letter,go));
	}

	IEnumerator doLetterAnimation(Letter letter,GameObject finalPosObj)
	{
		yield return new WaitForSeconds (0);

		letter.transform.DOMove ( finalPosObj.transform.position, selectAnimationTime).OnComplete(()=>{addLetterToContainer(letter); releaseChild(finalPosObj); reacomodateChildrenSiblingOrder();}).SetId(letter.GetInstanceID());
	}

	protected void reacomodateChildrenSiblingOrder()
	{
		for(int i=0; i<occupiedChildren.Count; i++)
		{
			occupiedChildren[i].transform.SetAsLastSibling();
		}
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

	public List<Letter> findLetters(List<Letter> pool)
	{
		List<ABCChar> charPool = new List<ABCChar>(pool.Count);
		List<Letter> wordLettersFound = new List<Letter> ();

		foreach(Letter l in pool)
		{
			charPool.Add(l.abcChar);
		}
		charPool = wordsValidator.getPosibleWord (charPool);

		for(int j=0; j<charPool.Count; j++)
		{
			for(int i=0; i<pool.Count; i++)
			{
				if(pool[i].abcChar == charPool[j])
				{
					wordLettersFound.Add (pool [i]);
					break;
				}
			}
		}

		return wordLettersFound;
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
			if(letterContainerTransform.GetChild(i).name != "stock")
			{
				letters.Add (letterContainerTransform.GetChild (i).GetComponent<Letter> ());
			}
		}
		for(int i=0; i<preLetterContainerTransform.childCount; i++)
		{
			letters.Add(preLetterContainerTransform.GetChild(i).GetComponent<Letter>());
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
		bool letterOnContainer = isThereAnyLetterOnContainer ();

		activateWordBtn (completeWord,letterOnContainer);
		activatePointsGO(letterOnContainer);
		activateNoWordPosibleText (!letterOnContainer,letterOnContainer);

		if(completeWord && letterOnContainer)
		{
			Debug.Log("Se completo: "+getCurrentWordOnList());
		}
	}

	public void activateWordBtn(bool completeWord, bool isThereAnyLetterOnContainer)
	{
		float speed = 0.25f;
		DOTween.Kill (wordDeleteButton,true);
		DOTween.Kill (wordCompleteButton,true);

		//wordCompleteButtonRectTransform.anchoredPosition = new Vector2 (Screen.width * 0.5f, wordCompleteButtonRectTransform.anchoredPosition.y);
		//wordDeleteButtonRectTransform.anchoredPosition = new Vector2 (Screen.width * 0.5f, wordDeleteButtonRectTransform.anchoredPosition.y);
		if(completeWord && isThereAnyLetterOnContainer)
		{
			if(wordDeleteButton.activeSelf)
			{
				wordCompleteButtonRectTransform.localScale = Vector2.zero;
				wordDeleteButtonRectTransform.DOScale (Vector2.zero, speed).SetId(wordDeleteButton).OnComplete(()=>
					{
						wordDeleteButton.SetActive (false);
						wordCompleteButton.SetActive (true);
						wordCompleteButtonRectTransform.DOScale(new Vector2(1,1),speed).SetId(wordCompleteButton);
					});
			}
			else if(wordCompleteButton.activeSelf)
			{
				
			}				
			else
			{
				wordDeleteButton.SetActive (false);
				wordCompleteButton.SetActive (true);
				wordCompleteButtonRectTransform.anchoredPosition = new Vector2 (Screen.width * speed, wordCompleteButtonRectTransform.anchoredPosition.y);
				wordCompleteButtonRectTransform.localScale = new Vector2 (1, 1);
				wordCompleteButtonRectTransform.DOAnchorPos (Vector2.zero, speed);
			}
		}
		else if(isThereAnyLetterOnContainer)
		{
			if(wordCompleteButton.activeSelf)
			{
				wordDeleteButtonRectTransform.localScale = Vector2.zero;
				wordCompleteButtonRectTransform.DOScale (Vector2.zero, speed).SetId(wordCompleteButton).OnComplete(()=>
					{
						wordDeleteButton.SetActive (true);
						wordCompleteButton.SetActive (false);
						wordDeleteButtonRectTransform.DOScale(new Vector2(1,1),speed).SetId(wordDeleteButton);
					});
			}
			else if(wordDeleteButton.activeSelf)
			{
				
			}
			else
			{
				wordDeleteButton.SetActive (true);
				wordCompleteButton.SetActive (false);
				wordDeleteButtonRectTransform.anchoredPosition = new Vector2 (Screen.width * speed, wordDeleteButtonRectTransform.anchoredPosition.y);
				wordDeleteButtonRectTransform.localScale = new Vector2 (1, 1);
				wordDeleteButtonRectTransform.DOAnchorPos (Vector2.zero, speed);
			}
		}
		else
		{
			wordCompleteButton.SetActive (false);
			wordDeleteButton.SetActive (false);
		}
	}

	public void activatePointsGO(bool active)
	{
		points.SetActive (active);
	}

	public void activateNoWordPosibleText(bool activate,bool isThereAnyLetterOnContainer = true)
	{
		if(currentWordPosibleState == EWordState.NO_WORDS_AVAILABLE && !isThereAnyLetterOnContainer)
		{
			noWordPosible.SetActive (true);
		}
		else
		{
			noWordPosible.SetActive (false);
		}
		//noWordPosible.SetActive (activate);
	}

	private void onLettersChange()
	{
		updateWordPoints();

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
			//deleteButtonImage.sprite = deleteWordState;
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

		if (wordsValidator == null) 
		{
			wordsValidator = PersistentData.GetInstance ().abcDictionary;
		}

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
		abc.wildcard = false;
		abc.character = wildcardValue;
		abc.pointsOrMultiple = pointsOrMultiple;

		letter.type = Letter.EType.WILD_CARD;
		letter.updateSprite ();
		letter.abcChar = abc;

		letter.updateTexts();
		letter.wildCard = true;

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
		letter.updateSprite();

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
		ABCChar abc = new ABCChar ();

		abc.wildcard = false;
		abc.character = character;
		abc.pointsOrMultiple = "x3";
		abc.value = wordsValidator.getCharValue(character);

		wildCard.wildCard = true;

		wildCard.initializeFromABCChar (abc);

		wildCard.type = Letter.EType.WILD_CARD;

		wildCard.updateTexts();
		wildCard.updateSprite ();

		saveAndValidateLetter(wildCard);
		resetValidationToSiblingOrder ();
		afterWordValidation ();
	}

	public void updateGridLettersState(List<Letter> gridLetter,EWordState wordState)
	{
		switch (wordState) {
		case EWordState.NO_WORDS_AVAILABLE:
			if(currentWordPosibleState != EWordState.NO_WORDS_AVAILABLE)
			{
				currentWordPosibleState = EWordState.NO_WORDS_AVAILABLE;
				for(int i=0; i<gridLetter.Count; i++)
				{
					gridLetter [i].updateState (Letter.EState.WRONG);
				}
				noWordPosible.SetActive (true);
				activateNoWordPosibleText (true);
			}
			break;
		case EWordState.WORDS_AVAILABLE:
			if(currentWordPosibleState != EWordState.WORDS_AVAILABLE)
			{
				currentWordPosibleState = EWordState.WORDS_AVAILABLE;
				for(int i=0; i<gridLetter.Count; i++)
				{
					gridLetter [i].updateState (Letter.EState.NORMAL);
				}
				activateNoWordPosibleText (false);
			}
			noWordPosible.SetActive (false);
			break;
		case EWordState.HINTED_WORDS:
			if (currentWordPosibleState != EWordState.HINTED_WORDS && !cancelHint) 
			{
				currentWordPosibleState = EWordState.HINTED_WORDS;
				StartCoroutine (updateLetterHintState (gridLetter));
			}
			break;
		}

	}

	IEnumerator updateLetterHintState(List<Letter> gridLetter)
	{
		yield return new WaitForSeconds (0);
		cancelHint = false;
		for(int i=0; i<gridLetter.Count; i++)
		{
			yield return new WaitForSeconds (0.4f);
			if(!cancelHint)
			{
				gridLetter [i].updateState (Letter.EState.HINTED);	
				yield return new WaitForSeconds (0.4f);
			}
		}

		if(!cancelHint)
		{
			currentWordPosibleState = EWordState.HINTED_WORDS;
			updateGridLettersState (gridLetter, EWordState.WORDS_AVAILABLE);
			updateGridLettersState (gridLetter, EWordState.HINTED_WORDS);	
		}
	}

	public void cancelHinting(List<Letter> gridLetter)
	{
		cancelHint = true;
		updateGridLettersState (gridLetter, EWordState.WORDS_AVAILABLE);
	}

	public void animateWordRetrieved(Letter letter)
	{
		Canvas canvas =  letter.GetComponent<Canvas>();
		Transform letterTransform = letter.transform;

		letterTransform.SetParent (LetterAnimatedContainerTransform,false);
		letterTransform.DOLocalRotate(new Vector3(0,0,360),1.5f, RotateMode.FastBeyond360);

		lettersRemoval.Add (letter);
		//letters.Remove(letter);
		StartCoroutine (lettersRemove(letter));
		
		letterTransform.DOLocalMoveY(25,0.5f);
		letterTransform.DOLocalMoveX (40, 0.5f).OnComplete(()=>
			{
				canvas.sortingLayerName = "UI";
				canvas.sortingOrder = -1;
				letterTransform.DOLocalMoveY(100,1f);
				letterTransform.DOScale(new Vector3(0,0,0),1f).OnComplete(()=>
					{
						activateGridLayout (true);
						DestroyImmediate(letter.gameObject);
						lettersRemoval.Remove (letter);
					});
			});
	}

	IEnumerator lettersRemove(Letter l)
	{
		yield return new WaitForEndOfFrame();
		letters.Remove (l);
	}

	public IEnumerator afterAllLettersRemoved()
	{
		yield return new WaitForSeconds (1.51f);
		onLettersChange ();
	}
}