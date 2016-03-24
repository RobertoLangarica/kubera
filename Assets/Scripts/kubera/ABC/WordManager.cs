using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ABC
{
	public class WordManager : MonoBehaviour 
	{

		public enum EDeleteState
		{
			WORD,CHARACTER
		}

		public GameObject letterPrefab;
		public GameObject letterContainer;

		public GameObject wordCompleteButton;
		public Image deleteButtonImage;
		public Sprite deleteCharacterState;
		public Sprite deleteWordState;

		public int maxLetters = 10;

		protected InputWords inputWords;

		[HideInInspector]public ABCDictionary wordsValidator;
		public List<Letter> letters;

		protected int siblingIndexAfterSwap;
		protected Vector2[] lettersPositions; //los vectores de las letras 

		protected float letterPrefabHeight = 0;

		[HideInInspector]public Vector3 deleteBtnPosition;

		public delegate void DSendVector3(Vector3 vector3);
		public DSendVector3 OnSendVector3;

		protected GridLayoutGroup gridLayoutGroup;

		public int wordPoints;

		void Start()
		{
			letters = new List<Letter>(maxLetters);

			deleteBtnPosition = deleteButtonImage.transform.localPosition;

			//Tamaño de las celdas
			gridLayoutGroup = letterContainer.GetComponent<GridLayoutGroup>();
			RectTransform rectT = letterContainer.GetComponent<RectTransform> ();
			float cellWidth =  (rectT.rect.width - gridLayoutGroup.padding.left 
									- gridLayoutGroup.padding.right - (gridLayoutGroup.spacing.x * (maxLetters-1)) )   /maxLetters;

			if(cellWidth < rectT.rect.height *.9f)
			{
				gridLayoutGroup.cellSize = new Vector2(cellWidth,cellWidth);
			}
			else
			{
				gridLayoutGroup.cellSize = new Vector2(rectT.rect.height*0.9f, rectT.rect.height*0.9f);
			}

			wordsValidator = FindObjectOfType<ABCDictionary>();

			inputWords = FindObjectOfType<InputWords> ();

			if(inputWords)
			{
				inputWords.onTap		+= OnGridLetterTapped;
				inputWords.onTapToDelete+= onLetterTap;
				inputWords.onDragUpdate += OnLettersSwapping;
				inputWords.onDragFinish += OnSwappEnding;
				inputWords.onDragStart  += OnActivateSwapp;
			}

			activateWordDeleteBtn(false);
			activateWordCompleteBtn(false);
		}

		public void OnGridLetterTapped(GameObject go)
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

		public void onLetterTap(GameObject go)
		{
			Letter letter = go.GetComponent<Letter>();
			removeLetter(letter.letterReference);
		}

		protected void OnActivateSwapp(GameObject target)
		{
			activateGridLayout (false);
			fillLettersPositions ();
			siblingIndexAfterSwap = target.transform.GetSiblingIndex();
			setSiblingIndex (target, maxLetters);
			changeDeleteState(EDeleteState.CHARACTER);
		}

		protected void fillLettersPositions()
		{
			Transform container = letterContainer.transform;
			lettersPositions = new Vector2[container.childCount];
			for(int i=0; i< container.childCount; i++)
			{
				lettersPositions[i] = container.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
			}
		}

		public void OnLettersSwapping(GameObject letter)
		{
			Transform container = letterContainer.transform;
			for(int i = 0; i< container.childCount; i++)
			{			
				if(container.GetChild(i).gameObject != letter)
				{
					RectTransform childRect = container.GetChild(i).GetComponent<RectTransform>();
					RectTransform letterRect = letter.GetComponent<RectTransform>();

					if( (int)childRect.anchoredPosition.x > (int)(letterRect.anchoredPosition.x - (letterRect.rect.width * 0.5f) ) 
						&& (int)childRect.anchoredPosition.x < (int)( letterRect.anchoredPosition.x + (letterRect.rect.width * 0.5f)) )
					{
						if(letterRect.anchoredPosition.x > childRect.anchoredPosition.x)
						{
							//izquierda a derecha
							siblingIndexAfterSwap = i;

							for(int j=container.childCount-2; j>=i; j--)
							{
								container.GetChild(j).GetComponent<RectTransform>().anchoredPosition = lettersPositions[j+1];
							}
						}
						else
						{
							//derecha a izquierda 
							siblingIndexAfterSwap = i+1;

							for(int j =0; j<=i; j++)
							{
								container.GetChild(j).GetComponent<RectTransform>().anchoredPosition = lettersPositions[j];
							}
						}
					}

					break;
				}
			}

		}

		protected void OnSwappEnding(GameObject target)
		{
			Letter letter = target.GetComponent<Letter>();

			//Los comodines no se pueden destruir
			if(!letter.abcChar.wildcard && isOverDeleteArea(letter.transform.localPosition))
			{
				removeLetter(letter);
			}
			else
			{
				setSiblingIndex (letter.gameObject, siblingIndexAfterSwap);
			}

			activateGridLayout (true);
			sortLettersAfterSwipe();
			validateAllLetters();
			changeDeleteState(EDeleteState.WORD);
		}

		protected bool isOverDeleteArea(Vector3 target)
		{
			//TODO: Ver si min y max hacen la chamba de esas restas
			if( target.x > (deleteBtnPosition.x - (deleteButtonImage.rectTransform.rect.width*0.5f) ) 
				&& target.x < (deleteBtnPosition.x + (deleteButtonImage.rectTransform.rect.width*0.5f) )  )
			{
				return true;	
			}

			return false;
		}

		protected void setSiblingIndex(GameObject target, int siblingPosition)
		{
			target.transform.SetSiblingIndex (siblingPosition);
		}

		protected void sortLettersAfterSwipe()
		{
			resetValidationToSiblingOrder();

			if(!isThereAnyLetterOnContainer())
			{
				
			}

			onLettersChange();
		}
			
		//TODO: Posiblemente se necesita que devuelva Letter
		public Letter getWildcard(string pointsOrMultiple)
		{			
			//TODO: Usar el prefab de letras
			GameObject result = new GameObject();
			Letter letter = result.AddComponent<Letter> ();
			string wildcardValue = ".";
			ABCChar abc = new ABCChar();

			abc.value = wordsValidator.getCharValue(wildcardValue);
			abc.wildcard = true;
			abc.character = wildcardValue;
			abc.pointsOrMultiple = pointsOrMultiple;
			letter.type = Letter.EType.NORMAL;

			return letter;
		}

		public void addLetterFromGrid(Letter gridReference)
		{
			if(isAddLetterAllowed())
			{
				//Clone para la visualizacion en WordManager
				Letter clone = Instantiate(letterPrefab).GetComponent<Letter>();
				clone.transform.localScale = new Vector3 (1, 1, 1);
				clone.abcChar = gridReference.abcChar;
				clone.type = gridReference.type;
				clone.letterReference = gridReference;
				clone.updateTexts();
				gridReference.letterReference = clone;

				addLetter(clone);
			}
		}

		public bool isAddLetterAllowed()
		{
			return letters.Count < maxLetters;
		}

		public void addLetter(Letter letter)
		{
			addLetterToValidationList(letter);
			addLetterToContainer(letter);
		}

		protected void addLetterToValidationList(Letter letter)
		{
			if(letters.Count == 0)
			{
				wordsValidator.initCharByCharValidation();
			}

			letters.Add(letter);
			wordsValidator.validateChar(letter.abcChar);

			afterWordValidation();

			onLettersChange();
		}

		public void removeAllLetters(bool includeWildcards = false)
		{
			int count = letters.Count;
			while(count >= 0)
			{
				--count;
				letters[count].deselect();
				GameObject.DestroyImmediate(letters[count].gameObject);
				letters.RemoveAt(count);
			}

			if(!includeWildcards)
			{
				resetValidationToSiblingOrder();
			}

			onLettersChange();
		}

		private void removeLetter(Letter letter)
		{
			letters.Remove(letter);

			letter.deselect();
			GameObject.DestroyImmediate(letter.gameObject);

			onLettersChange();
		}

		protected void afterWordValidation()
		{
			activateWordCompleteBtn(wordsValidator.isCompleteWord());

			if(wordsValidator.isCompleteWord())
			{
				Debug.Log("Se completo: "+getCurrentWordOnList());
			}
		}

		protected void addLetterToContainer(Letter letter)
		{
			//Agregamos la letra al ultimo
			letter.transform.SetParent(letterContainer.transform,false);

			//TODO: Porque se hace este resize
			updateLetterBoxCollider (letter.gameObject);
		}

		protected void updateLetterBoxCollider(GameObject letter)
		{
			StartCoroutine(resizeBoxCollider(letter));
		}

		IEnumerator resizeBoxCollider(GameObject letter)
		{
			yield return new WaitForSeconds (0.1f);
			letter.GetComponent<BoxCollider2D> ().size = letter.GetComponent<Image> ().rectTransform.rect.size;
		}

		protected bool isThereAnyLetterOnContainer()
		{
			return (letterContainer.transform.childCount == 0 ? false:true); 
		}

		public string getCurrentWordOnList()
		{
			string result = "";

			//character es desconfiable por los comodines (usamos value)
			foreach(Letter l in letters)
			{
				result = result+wordsValidator.getStringByValue(l.abcChar.value);
			}

			return result;
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

		protected void activateGridLayout(bool activate)
		{
			gridLayoutGroup.enabled = activate;
		}
			
		protected void resetValidationToSiblingOrder ()
		{
			letters.Clear();

			for(int i=0; i<letterContainer.transform.childCount; i++)
			{
				letters.Add(letterContainer.transform.GetChild(i).GetComponent<Letter>());
			}
		}

		protected void validateAllLetters()
		{
			wordsValidator.initCharByCharValidation();

			foreach(Letter l in letters)
			{
				wordsValidator.validateChar(l.abcChar);
			}

			afterWordValidation();
		}
			
			
		public void activateWordDeleteBtn(bool active)
		{
			deleteButtonImage.gameObject.SetActive(active);
		}

		public void activateWordCompleteBtn(bool active)
		{
			wordCompleteButton.SetActive (false);
		}

		public void changeDeleteState(EDeleteState state)
		{
			switch (state) 
			{
			case EDeleteState.WORD:
				deleteButtonImage.sprite = deleteWordState;
				break;
			case EDeleteState.CHARACTER:
				deleteButtonImage.sprite = deleteCharacterState;
				break;
			}
		}

		protected void onLettersChange()
		{
			updateWordPoints();

			if(!isThereAnyLetterOnContainer())
			{
				activateWordDeleteBtn(false);	
			}
			else
			{
				changeDeleteState(EDeleteState.WORD);		
			}
		}

		protected void updateWordPoints()
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
	}
}
