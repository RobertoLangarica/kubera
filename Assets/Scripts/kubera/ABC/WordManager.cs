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
		public GameObject emptyChild;
		public GameObject letterContainer;

		public GameObject wordCompleteButton;
		public Image deleteButtonImage;
		public Sprite deleteCharacterState;
		public Sprite deleteWordState;

		public int maxLetters = 10;

		protected InputWords inputWords;

		[HideInInspector]public ABCDataStructure wordsValidator;
		public List<ABCChar> chars;
		protected bool invalidCharlist;//Indica que la lista de caracteres tuvo o tiene uno invalido

		protected int sortingOrderAfterSwapp;
		protected Vector2[] lettersPositions; //los vectores de las letras 

		protected float letterPrefabHeight = 0;

		[HideInInspector]public Vector3 deleteBtnPosition;

		public delegate void DSendVector3(Vector3 vector3);
		public DSendVector3 OnSendVector3;

		public delegate void DLettersActualized();
		public DLettersActualized OnLettersActualized;

		protected GridLayoutGroup gridLayoutGroup;
		void Start()
		{
			inputWords = FindObjectOfType<InputWords> ();

			if(inputWords)
			{
				inputWords.onDragUpdate += OnLettersSwapping;
				inputWords.onDragFinish += OnSwappEnding;
				inputWords.onDragStart  += OnActivateSwapp;
				inputWords.onTap += addLetterToWord;
				inputWords.onTapAfterLongPress += destroyLetterAfterLongPress;
			}

			chars = new List<ABCChar>();

			deleteBtnPosition = deleteButtonImage.transform.localPosition;

			gridLayoutGroup = letterContainer.GetComponent<GridLayoutGroup>();

			if(((letterContainer.GetComponent<RectTransform> ().rect.width/maxLetters )-gridLayoutGroup.padding.left) < letterContainer.GetComponent<RectTransform> ().rect.height *.8f)
			{
				gridLayoutGroup.cellSize = new Vector2((letterContainer.GetComponent<RectTransform> ().rect.width/maxLetters )-5
					,(letterContainer.GetComponent<RectTransform> ().rect.width/maxLetters )-gridLayoutGroup.padding.left);
			}
			else
			{
				gridLayoutGroup.cellSize = new Vector2(letterContainer.GetComponent<RectTransform>().rect.height*.9f
					,letterContainer.GetComponent<RectTransform>().rect.height*.9f);
			}



			wordsValidator = FindObjectOfType<ABCDataStructure>();

			activateWordDeleteButton (false);
			wordCompleteButton.SetActive (false);
		}
			

		public GameObject getWildcard(string pointsOrMultiple)
		{			
			GameObject result = new GameObject();
			result.AddComponent<ABCChar> ();
			string wildcardValue = ".";
			result.GetComponent<ABCChar>().value = wordsValidator.getCharValue(wildcardValue);
			result.GetComponent<ABCChar>().wildcard = true;
			result.GetComponent<ABCChar>().character = wildcardValue;
			result.GetComponent<ABCChar>().pointsOrMultiple = pointsOrMultiple;
			result.GetComponent<ABCChar>().type = ABCChar.EType.NORMAL;

			return result;
		}

		public void addCharacter(ABCChar pieceABCChar,GameObject piece)
		{
			GameObject letter =  Instantiate(letterPrefab);
			ABCChar character = letter.GetComponent<ABCChar>();

			character.wildcard = pieceABCChar.wildcard;
			character.value = wordsValidator.getCharValue(pieceABCChar.character.ToUpper());
			character.character = pieceABCChar.character.ToUpperInvariant();
			character.pointsOrMultiple = pieceABCChar.pointsOrMultiple;
			character.type = pieceABCChar.type;
			
			addLetterToFirstEmptySpace(letter);

			letter.transform.localScale = new Vector3 (1, 1, 1);
			letter.GetComponent<WordChar> ().gridLetterReference = piece;
			if (piece.GetComponent<WordChar> ()) 
			{
				piece.GetComponent<WordChar> ().gridLetterReference = letter;
			}
				
			letter.GetComponent<ABCChar>().initializeText();
			validateCharacter(character);
		}

		public void lettersActualized()
		{
			OnLettersActualized ();
		}

		protected bool isThereAnyLetterOnContainer()
		{
			return (letterContainer.transform.childCount == 0 ? false:true); 
		}

		protected void actualizeBoxColliderOfLetter(GameObject letter)
		{
			StartCoroutine(actualizeBoxCollider(letter));
		}

		IEnumerator actualizeBoxCollider(GameObject letter)
		{
			yield return new WaitForSeconds (0.1f);
			letter.GetComponent<BoxCollider2D> ().size = letter.GetComponent<Image> ().rectTransform.rect.size;
		}

		/**
		 * Agrega la siguiente letra tomando en cuenta los espacios vacios
		 **/ 
		protected void addLetterToFirstEmptySpace(GameObject letter)
		{
			//Agregamos la letra al primer lugar vacio
			for(int i = 0; i < chars.Count; i++)
			{
				if(chars[i].empty)
				{
					DestroyImmediate(letterContainer.transform.GetChild(i).gameObject);
					letter.transform.SetParent(letterContainer.transform);
					setSibilingIndex (letter, i);
					return;
				}
			}

			//Agregamos la letra al ultimo
			letter.transform.SetParent(letterContainer.transform,false);

			actualizeBoxColliderOfLetter (letter);
		}

		/**
		 * Valida el caracter para ver si ya se completo una palabra
		 **/ 
		protected void validateCharacter(ABCChar character)
		{
			if(chars.Count == 0)
			{
				wordsValidator.initCharByCharValidation();
			}

			if(invalidCharlist)
			{
				int index = getFirstEmptyIndex();

				//Sustituimos el character
				chars[index].empty = false;
				chars[index] = character;
				character.index = index;

				if(!hasEmptyChars())
				{
					//Se completaron los vacios y los validamos
					wordsValidator.initCharByCharValidation();

					foreach(ABCChar c in chars)
					{
						wordsValidator.validateChar(c);
					}

					invalidCharlist = false;

					if(wordsValidator.isCompleteWord())
					{
						onWordComplete(true);
					}
					else
					{
						changeDeleteState(0);
						onWordComplete(false);
					}
				}
			}
			else
			{
				character.index = chars.Count;
				chars.Add(character);
				wordsValidator.validateChar(character);

				if(wordsValidator.isCompleteWord())
				{
					onWordComplete(true);
				}
				else
				{
					changeDeleteState(0);
					onWordComplete(false);
				}
			}
		}


		/**
		 * Elimina los caracteres de la busqueda actual
		 **/ 
		public void resetValidation(bool reset = false)
		{
			if (letterContainer.transform.childCount != 0) 
			{
				for (int i = 0; i < chars.Count; i++)
				{
					ABCChar abcChar;
					
					if (chars [i].gameObject.GetComponent<WordChar> ().gridLetterReference != null) 
					{
						abcChar = chars [i].gameObject.GetComponent<WordChar> ().gridLetterReference.GetComponent<ABCChar> ();
					} 
					else 
					{
						abcChar = chars [i].gameObject.GetComponent<ABCChar> ();
					}

					WordChar uiChar = chars [i].gameObject.GetComponent<WordChar> ();
					if (!reset) 
					{
						if (uiChar != null && abcChar != null) 
						{
							if (wordsValidator.isCompleteWord()) 
							{
								if (!abcChar.wildcard) 
								{
									OnSendVector3 (uiChar.gridLetterReference.transform.position);
									uiChar.destroyLetterFromGrid ();
								}
								else 
								{
									DestroyImmediate (uiChar.gameObject);
								}
							} 
							else 
							{
								if (!abcChar.wildcard) 
								{
									uiChar.gridLetterReference.GetComponent<WordChar> ().markAsUnselected ();
								}

							}
						}
					}
					else
					{
						if (!abcChar.wildcard) 
						{
							uiChar.gridLetterReference.GetComponent<WordChar> ().markAsUnselected ();
						}
					}
				}
			}

			invalidCharlist = false;

			int l = letterContainer.transform.childCount;
			while(--l >= 0)
			{
				if (!letterContainer.transform.GetChild (l).gameObject.GetComponent<ABCChar> ().wildcard) 
				{
					GameObject.DestroyImmediate (letterContainer.transform.GetChild (l).gameObject);
				}
			}

			foreach(ABCChar c in chars)
			{
				c.empty = false;
			}

			resetWordValidationToSiblingOrder ();

			lettersActualized ();
		}

		/**
		 * Cuando se completa una palabra
		 **/ 
		protected void onWordComplete(bool wordCompleted)
		{
			if(wordCompleted)
			{
				Debug.Log("Se completo: "+getFullWord());
				wordCompleteButton.SetActive (true);
			}
			else
			{
				wordCompleteButton.SetActive (false);
			}

		}

		/**
		 * Devuelve la palabra que se esta validando en ese momento
		 * */
		public string getFullWord()
		{
			string result = "";

			//characte es desconfiable por los comodines (usamos value)
			foreach(ABCChar c in chars)
			{
				result = result+wordsValidator.getStringByValue(c.value);
			}

			return result;
		}

		/**
		 * elimina el caracter indicado de la busqueda
		 * */
		public void deleteCharFromSearch(int lvlIndex)
		{
			if(lvlIndex == chars.Count-1)
			{
				//El usuairo elimino el ultimo caracter
				wordsValidator.deleteLvlOfSearch(lvlIndex);
				chars.RemoveAt(lvlIndex);
			}
			else
			{
				wordsValidator.cleanCharByCharValidation();
				chars[lvlIndex].empty = true;
				invalidCharlist = true;
			}

			//Eliminamos la letra anterior
			DestroyImmediate(letterContainer.transform.GetChild(lvlIndex).gameObject);

			
			if(everythingIsEmpty())
			{
				resetValidation();
				//container.GetComponent<HorizontalLayoutGroup>().padding.left = container.GetComponent<HorizontalLayoutGroup>().padding.right = padding = 300;
			}
			else if(lvlIndex != chars.Count)
			{
				//agregar hijo vacio para indicar el espacio
				GameObject letter =  Instantiate(emptyChild);

				letter.transform.SetParent(letterContainer.transform);
				setSibilingIndex (letter, lvlIndex);
				letter.transform.localScale = new Vector3(1,1,1);
			}
		}

		public void deleteCharFromSearch(ABCChar abcChar)
		{
			deleteCharFromSearch(abcChar.index);
		}

		/**
		 * Indica si todos los caracteres en la busqueda los elimino 
		 * el usuario y se marcaron como empty = true
		 * */
		protected bool everythingIsEmpty()
		{
			foreach(ABCChar c in chars)
			{
				if(!c.empty)
				{
					return false;
				}
			}
			return true;
		}

		/**
		 * indica si la lista tiene caracteres vacios
		 **/ 
		protected bool hasEmptyChars()
		{	
			foreach(ABCChar c in chars)
			{
				if(c.empty)
				{
					return true;
				}
			}
			return false;
		}
		/**
		 * Obtiene el indice del primer caracter vacio dentro de la lista
		 **/ 
		protected int getFirstEmptyIndex()
		{
			for(int i = 0; i < chars.Count; i++)
			{
				if(chars[i].empty)
				{
					return i;
				}
			}

			return -1;
		}

		/**
		 * Revisa si es posible armar una palabra con los caracteres que se tienen en este momento
		 **/ 
		public void checkIfAWordIsPossible()
		{
			checkIfAWordIsPossible(chars);
		}

		/**
		 * Revisa si es posible armar una palabra con los caracteres que se tienen
		 **/
		public bool checkIfAWordIsPossible(List<ABCChar> pool)
		{
			//TODO: checar si realmente no puede hacer una palabra
			//Debug.Log ("Possible word: "+words.isAWordPossible(pool));
			if(!wordsValidator.isAWordPossible(pool))
			{
				print("perdio de verdad");
				return false;
			}
			return true;
		}

		protected void OnActivateSwapp(GameObject letter)
		{
			activateGridLayout (false);
			fillLettersPositions ();
			sortingOrderAfterSwapp = letter.transform.GetSiblingIndex();
			setSibilingIndex (letter, maxLetters);
			changeDeleteState(EDeleteState.CHARACTER);
		}


		public void OnLettersSwapping(GameObject letter)
		{
			for(int i=0; i<letterContainer.transform.childCount; i++)
			{			
				if(letterContainer.transform.GetChild(i).gameObject != letter)
				{
					if((int)letterContainer.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x > ((int)letter.GetComponent<RectTransform>().anchoredPosition.x - ((int)letter.GetComponent<RectTransform>().rect.width * 0.5f)) 
					   && (int)letterContainer.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x < ((int)letter.GetComponent<RectTransform>().anchoredPosition.x + ((int)letter.GetComponent<RectTransform>().rect.width * 0.5f)))
					{
						if(letter.GetComponent<RectTransform>().anchoredPosition.x > letterContainer.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition.x)
						{
							//izquierda a derecha
							sortingOrderAfterSwapp = i;

							for(int j=letterContainer.transform.childCount-2; j>=i; j--)
							{
								letterContainer.transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = lettersPositions[j+1];
							}
						}
						else
						{
							//derecha a izquierda 
							sortingOrderAfterSwapp = i+1;

							for(int j =0; j<=i; j++)
							{
								letterContainer.transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = lettersPositions[j];
							}
						}
						break;
					}
				}
			}

		}
			
		/**
		* activa o desactiva el poder mover las letras
		* a la letra que se movera se mueve su index para que este arriba de las otras letras
		* destruye la letra seleccionada si la arrojaron a la basura
		**/
		protected void OnSwappEnding(GameObject letter)
		{
			//Los comodines nos ep ueden destruir
			if(!letter.GetComponent<ABCChar> ().wildcard && isOverDeleteArea(letter.transform.localPosition))
			{
				destroyLetter (letter);
			}
			else
			{
				setSibilingIndex (letter, sortingOrderAfterSwapp);
			}

			activateGridLayout (true);

			afterLettersChange ();
		}

		protected void afterLettersChange()
		{
			resetWordValidationToSiblingOrder();

			if(!isThereAnyLetterOnContainer())
			{
				activateWordDeleteButton(false);
			}
			lettersActualized ();
		}

		protected bool isOverDeleteArea(Vector3 target)
		{
			//DONE: Esos 50 son hardcoding? o de donde viene ese rango
			if(target.x > (deleteBtnPosition.x - (deleteButtonImage.rectTransform.rect.width*0.5f)) &&
				target.x < (deleteBtnPosition.x + (deleteButtonImage.rectTransform.rect.width*0.5f)))
			{
				return true;	
			}

			return false;
		}

		protected void destroyLetter(GameObject letter)
		{
			letter.GetComponent<WordChar> ().destroy();
		}

		protected void activateGridLayout(bool activate)
		{
			gridLayoutGroup.enabled = activate;
		}
			
		protected void resetWordValidationToSiblingOrder ()
		{
			chars.Clear();

			for(int i=0; i<letterContainer.transform.childCount; i++)
			{
				chars.Add(letterContainer.transform.GetChild(i).GetComponent<ABCChar>());
			}
			wordsValidator.initCharByCharValidation();

			foreach(ABCChar c in chars)
			{
				wordsValidator.validateChar(c);
			}

			if(wordsValidator.isCompleteWord())
			{
				onWordComplete(true);
			}
			else
			{
				onWordComplete(false);
			}
			changeDeleteState(EDeleteState.WORD);
		}

		/*
		 * llena el arreglo de vectores de las posiciones de las letras para poder moverlas
		 */
		protected void fillLettersPositions()
		{
			lettersPositions = new Vector2[letterContainer.transform.childCount];
			for(int i=0; i<letterContainer.transform.childCount; i++)
			{
				lettersPositions[i] = letterContainer.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
			}
		}
			
		public void addLetterToWord(GameObject go)
		{
			addLetterToWord(go.GetComponent<WordChar>());
		}

		public void addLetterToWord(WordChar letter)
		{
			if (!letter.isPreviouslySelected ()) 
			{
				if (maxLetters > letterContainer.transform.childCount) 
				{
					letter.markAsSelected ();
					addCharacter (letter.gameObject.GetComponent<ABCChar> (), letter.gameObject);
					activateWordDeleteButton(true);

					lettersActualized ();
				}
			}
			else
			{
				//DONE: Esto esta confuso, hay que tener una forma clara de agregar una nueva letra como ultimo sibling
				destroyLetter(letter.gridLetterReference);
				afterLettersChange ();
			}
		}

		public void destroyLetterAfterLongPress(GameObject go)
		{
			destroyLetter (go);
			afterLettersChange ();
		}

		public void activateWordDeleteButton(bool active)
		{
			deleteButtonImage.gameObject.SetActive(active);
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

		protected void setSibilingIndex(GameObject go, int siblingPosition)
		{
			go.transform.SetSiblingIndex (siblingPosition);
		}
	}
}
