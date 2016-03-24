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

		protected int siblingIndexAfterDrag;
		protected Vector2[] lettersPositions; //los vectores de las letras 

		protected float letterPrefabHeight = 0;

		[HideInInspector]public Vector3 deleteBtnPosition;

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

					 if( 	childRect.anchoredPosition.x > (letterRect.anchoredPosition.x - (letterRect.rect.width*0.5f) ) 
						&&	childRect.anchoredPosition.x < (letterRect.anchoredPosition.x + (letterRect.rect.width*0.5f) ) )
					{
						if(letterRect.anchoredPosition.x > childRect.anchoredPosition.x)
						{
							//izquierda a derecha
							siblingIndexAfterDrag = i;

							for(int j=container.childCount-2; j>=i; j--)
							{
								container.GetChild(j).GetComponent<RectTransform>().anchoredPosition = lettersPositions[j+1];
							}
						}
						else
						{
							//derecha a izquierda 
							siblingIndexAfterDrag = i+1;

							for(int j =0; j<=i; j++)
							{
								container.GetChild(j).GetComponent<RectTransform>().anchoredPosition = lettersPositions[j];
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
				removeLetter(letter);
			}
			else
			{
				setSiblingIndex (letter.gameObject, siblingIndexAfterDrag);
			}

			activateGridLayout (true);
			resetValidationToSiblingOrder();
			onLettersChange();
			validateAllLetters();
		}

		private bool isOverDeleteArea(Vector3 target)
		{
			//TODO: Ver si min y max hacen la chamba de esas restas
			if( target.x > (deleteBtnPosition.x - (deleteButtonImage.rectTransform.rect.width*0.5f) ) 
				&& target.x < (deleteBtnPosition.x + (deleteButtonImage.rectTransform.rect.width*0.5f) )  )
			{
				return true;	
			}

			return false;
		}

		private void setSiblingIndex(GameObject target, int siblingPosition)
		{
			target.transform.SetSiblingIndex (siblingPosition);
		}

			
		public Letter getWildcard(string pointsOrMultiple)
		{			
			//TODO: Usar el prefab de letras o un prefab de comodin
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

		public void removeLetter(Letter letter)
		{
			letters.Remove(letter);

			letter.deselect();
			GameObject.DestroyImmediate(letter.gameObject);

			onLettersChange();
		}

		private void afterWordValidation()
		{
			activateWordCompleteBtn(wordsValidator.isCompleteWord());

			if(wordsValidator.isCompleteWord())
			{
				Debug.Log("Se completo: "+getCurrentWordOnList());
			}
		}

		private void addLetterToContainer(Letter letter)
		{
			//Agregamos la letra al ultimo
			letter.transform.SetParent(letterContainer.transform,false);

			//TODO: Porque se hace este resize
			updateLetterBoxCollider (letter.gameObject);
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

		private bool isThereAnyLetterOnContainer()
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

		private void activateGridLayout(bool activate)
		{
			gridLayoutGroup.enabled = activate;
		}
			
		private void resetValidationToSiblingOrder ()
		{
			letters.Clear();

			for(int i=0; i<letterContainer.transform.childCount; i++)
			{
				letters.Add(letterContainer.transform.GetChild(i).GetComponent<Letter>());
			}
		}

		private void validateAllLetters()
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

		private void onLettersChange()
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
	}
}
