using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ABC
{
	public class WordManager : MonoBehaviour 
	{

		public GameObject letterPrefab;
		public GameObject emptyChild;
		public GameObject letterContainer;

		public GameObject wordActiveButton;
		public Sprite[] wordActiveButtonImagesStates;

		public int maxLetters = 12;

		protected InputWords inputWords;

		[HideInInspector]public ABCDataStructure wordsValidator;
		public List<ABCChar> chars;
		protected bool invalidCharlist;//Indica que la lista de caracteres tuvo o tiene uno invalido

		protected int sortingAfterSwap;
		protected Vector2[] lettersPositions; //los vectores de las letras 

		protected float letterPrefabHeight = 0;

		[HideInInspector]public Vector3 wordActiveButtonPosition;

		public delegate void DSendVector3(Vector3 vector3);
		public DSendVector3 OnSendVector3;

		void Start()
		{
			inputWords = FindObjectOfType<InputWords> ();

			if(inputWords)
			{
				inputWords.onDragUpdate += swappingLetters;
				inputWords.onDragFinish += swappEnding;
				inputWords.onDragStart  += activateSwapp;
				inputWords.onTap += sendLetterToWord;
			}

			chars = new List<ABCChar>();

			wordActiveButtonPosition = wordActiveButton.transform.localPosition;

			GridLayoutGroup gridLayout = letterContainer.GetComponent<GridLayoutGroup>();
			gridLayout.cellSize = new Vector2(letterContainer.GetComponent<RectTransform>().rect.width/maxLetters
				,letterContainer.GetComponent<RectTransform>().rect.height*.9f);


			wordsValidator = FindObjectOfType<ABCDataStructure>();

			wordActiveButton.SetActive(false);
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
			letter.GetComponent<UIChar> ().piece = piece;

			//letter.GetComponent<UIChar> ().changeImageTexture(changeTexture(character.character.ToLower () + "1"));
			letter.GetComponent<ABCChar>().initializeText();
			validateCharacter(character);
		}

		protected void isThereAnyLetterOnContainer()
		{
			if (letterContainer.transform.childCount == 0) 
			{
				activateButtonOfWordsActions (false);
			}
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
					letter.transform.SetSiblingIndex(i);
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

					if(wordsValidator.completeWord)
					{
						onWordComplete();
					}
					else
					{
						isCompletedNotCompletedOrMoving(1);
					}
				}
			}
			else
			{
				character.index = chars.Count;
				chars.Add(character);
				wordsValidator.validateChar(character);

				if(wordsValidator.completeWord)
				{
					onWordComplete();
				}
				else
				{
					isCompletedNotCompletedOrMoving(1);
				}
			}
		}


		/**
		 * Elimina los caracteres de la busqueda actual
		 **/ 
		public void resetValidation(bool correct = false)
		{
			if (letterContainer.transform.childCount != 0) 
			{
				for (int i = 0; i < chars.Count; i++)
				{
					ABCChar abcChar;
					if (chars [i].gameObject.GetComponent<UIChar> ().piece != null)
					{
						abcChar = chars [i].gameObject.GetComponent<UIChar> ().piece.GetComponent<ABCChar> ();
					} 
					else 
					{
						abcChar = chars [i].gameObject.GetComponent<ABCChar> ();
					}

					UIChar uiChar = chars [i].gameObject.GetComponent<UIChar> ();

					if (uiChar != null && abcChar != null) 
					{
						if (wordsValidator.completeWord) 
						{
							if (!abcChar.wildcard) 
							{
								OnSendVector3 (uiChar.piece.transform.position);
								uiChar.destroyPiece ();
							}
							else 
							{
								DestroyImmediate(uiChar.gameObject);
							}
						} 
						else 
						{
							if (!abcChar.wildcard) 
							{
								uiChar.piece.GetComponent<UIChar> ().backToNormal ();
							}

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

			actualizeIfExistLettersOn ();

		}

		/**
		 * Cuando se completa una palabra
		 **/ 
		protected void onWordComplete()
		{
			Debug.Log("Se completo: "+getFullWord());
			isCompletedNotCompletedOrMoving(0);
		}

		/**
		 * Devuelve la palabra que se esta validando en ese momento
		 * */
		public string getFullWord()
		{
			string result = "";

			foreach(ABCChar c in chars)
			{
				result = result+c.character;
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
				letter.transform.SetSiblingIndex(lvlIndex);
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
		public void checkIfAWordisPossible()
		{
			checkIfAWordisPossible(chars);
		}

		/**
		 * Revisa si es posible armar una palabra con los caracteres que se tienen
		 **/
		public bool checkIfAWordisPossible(List<ABCChar> pool)
		{
			//Debug.Log ("Possible word: "+words.isAWordPossible(pool));
			if(!wordsValidator.isAWordPossible(pool))
			{
				print("perdio de verdad");
				return false;
			}
			return true;
		}

		/**
		 *Cambia el orden de las letras 
		 **/
		public void swappingLetters(GameObject letter)
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
							sortingAfterSwap = i;

							for(int j=letterContainer.transform.childCount-2; j>=i; j--)
							{
								letterContainer.transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = lettersPositions[j+1];
							}
						}
						else
						{
							//derecha a izquierda 
							sortingAfterSwap = i+1;

							for(int j =0; j<=i; j++)
							{
								letterContainer.transform.GetChild(j).GetComponent<RectTransform>().anchoredPosition = lettersPositions[j];
							}
						}
						break;
					}
				}
			}
			isCompletedNotCompletedOrMoving(2);
		}

		/**
		 * checa si debe borrar la imagen y mandar a reacomodar
		 **/
		protected void swappEnding(GameObject letter)
		{
			if(!letter.GetComponent<ABCChar> ().wildcard && letter.transform.localPosition.x > wordActiveButtonPosition.x -50 && letter.transform.localPosition.x < wordActiveButtonPosition.x +50)
			{
				checkSwappLetters (letter, true);
			}
			else
			{
				checkSwappLetters(letter);	
			}
		}

		/**
		 * activa el poder mover las letras
		 **/
		protected void activateSwapp(GameObject letter)
		{
			letterContainer.GetComponent<GridLayoutGroup>().enabled = false;
			setPositionToLetters ();
			sortingAfterSwap = letter.transform.GetSiblingIndex();
			letter.transform.SetSiblingIndex(100);
		}

		/**
		  * activa o desactiva el poder mover las letras
		  * a la letra que se movera se mueve su index para que este arriba de las otras letras
		  * destruye la letra seleccionada si la arrojaron a la basura
		  **/
		public void checkSwappLetters(GameObject letter,bool destroy = false)
		{			
			letterContainer.GetComponent<GridLayoutGroup>().enabled = true;
			if(destroy)
			{
				letter.GetComponent<UIChar> ().destroyLetter();
			}
			else
			{
				letter.transform.SetSiblingIndex(sortingAfterSwap);
			}

			actualizeIfExistLettersOn ();

		}
			
		protected void actualizeIfExistLettersOn ()
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
			if(wordsValidator.completeWord)
			{
				onWordComplete();
			}
			else
			{
				isCompletedNotCompletedOrMoving(1);
			}
			isThereAnyLetterOnContainer ();
		}

		/*
		 * llena el arreglo de vectores de las posiciones de las letras para poder moverlas
		 */
		protected void setPositionToLetters()
		{
			lettersPositions = new Vector2[letterContainer.transform.childCount];
			for(int i=0; i<letterContainer.transform.childCount; i++)
			{
				lettersPositions[i] = letterContainer.transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
			}
		}

		/**
		 * Activar boton que aparece cuando ponen una letra y muestra si ya esta completa una palabra y al mover letras se convierte en bote de basura 
		 **/
		public void activateButtonOfWordsActions(bool activate)
		{
			activeWordActiveButton (activate);
		}

		/**
		 * Manda a llamar la letra
		 **/
		public void sendLetterToWord(GameObject go)
		{
			if (go.GetComponent<UIChar>().checkIfLetterCanBeUsedFromGrid ()) 
			{
				addCharacter (go.GetComponent<ABCChar> (), go);
				activateButtonOfWordsActions (true);
			}
		}

		public void activeWordActiveButton(bool showing)
		{
			wordActiveButton.SetActive(showing);
		}

		public void isCompletedNotCompletedOrMoving(int imageValue)
		{
			switch (imageValue) {
			case 0:
				wordActiveButton.GetComponent<Image> ().sprite = wordActiveButtonImagesStates [0];//completed
				break;
			case 1:
				wordActiveButton.GetComponent<Image> ().sprite = wordActiveButtonImagesStates [1];//notCompleted
				break;
			case 2:
				wordActiveButton.GetComponent<Image> ().sprite = wordActiveButtonImagesStates [2];//Moving
				break;
			default:
				break;
			}
		}
	}
}
