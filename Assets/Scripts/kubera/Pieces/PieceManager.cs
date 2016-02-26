using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PieceManager : MonoBehaviour 
{
	//CHANGE: pieces en plural es redundante con List asi que puede cambiar el nombre
	//CHANGE: una lista de piezas tiene significado para PieceManager y no me queda claro porque son GameObject
	//TODO: al parecer piecesList es la lista de las piezas que quedan, puede ser remainingPieces o availablePieces
	protected List<GameObject> piecesList = new List<GameObject>();
	//TODO: safeList es la lista de todas las piezas y podria llamarse asi pieces
	protected List<GameObject> safeList;

	//TODO: PieceManager no conoce una barra, puede tener una lista de piezas siendo mostradas showingPieces
	public List<Piece> piecesInBar = new List<Piece>();

	//TODO: Un nombre que indique que estas son las posiciones
	public Transform[] firstPos;

	//TODO: Hay que poner un nombre que indique que esta es la cantidad de piezas que se pueden mostrar
	//CHANGE: Que sea publica para poder editar esas piezas
	protected int sizeOfBar = 3;

	//UNDONE: Hardcoding
	protected Vector3 initialPieceScale = new Vector3(2.5f,2.5f,2.5f);
	//CHANGE: Cantidad de piezas que se estan mostrando, hay que ponerle un nombre que indique eso
	protected int piecesAvailable;
	//CHANGE: Es el contenedor de las piezas que se estan mostrando, podria llamarse showingPiecesContainer o algo parecido
	//TODO: que sea publico y que se asigne en el editor
	protected Transform piecesStock;

	public delegate void DOnShowMoney(bool show, int money);
	public DOnShowMoney OnShowMoney;

	//CHANGE: La rotacion es parte de un powerup, no creo que PieceManager tenga que hacer algo con eso
	public delegate void DOnRotate(bool rotating);
	public DOnRotate OnRotate;


	[HideInInspector]public bool isRotating = false;

	void Start () 
	{
		GameObject temp = new GameObject();
		piecesStock = temp.transform;
		temp.name = "PiecesStock";

		piecesAvailable = sizeOfBar;
	}
		
	//TODO: Esto inicializa las piezas que se van a mostrar, un mejor nombre
	protected void initializeAvailablePieces()
	{
		for(int i= 0; i<sizeOfBar; i++)
		{
			if(piecesList.Count==0)
			{
				fillList();
			}
			GameObject go = Instantiate(piecesList[0]);
			go.name = piecesList[0].name;

			piecesList.RemoveAt(0);

			//TODO: Que alguien mas
			go.transform.position= new Vector3(firstPos [i].position.x,firstPos [i].position.y,1);
			go.transform.localScale = new Vector3 (0, 0, 0);
			go.transform.DOScale(initialPieceScale, 0.25f);
			go.transform.SetParent (piecesStock);
			piecesInBar.Add(go.GetComponent<Piece>());
		}
		piecesAvailable = sizeOfBar;
	}

	//CHANGE: Una pool ya es una lista y se va hacer una lista de pools (o es lista o pool)
	//CHANGE: Si es una pool de piezas porque recibe GameObjects?
	public void setPiecesPoolList(List<GameObject> piecesPoolList)
	{
		safeList = new List<GameObject>();
		safeList = piecesPoolList;


		//TODO: GameManager va llamaar initializeAvailablePieces cuando lo necesite
		initializeAvailablePieces ();
	}

	//CHANGE: Al parecer fillList no llena la lista, randomiza la lista de piezas
	//TODO: Que sea un randomizelist y que reciba que lista randomizar para que sea reusable
	protected void fillList()
	{
		List<GameObject> newList = new List<GameObject>();
		piecesList = new List<GameObject>(safeList);

		while(piecesList.Count >0)
		{
			int val = UnityEngine.Random.Range(0,piecesList.Count);
			newList.Add(piecesList[val]);
			piecesList.RemoveAt(val);
		}

		piecesList = newList;
	}

	//TODO: Esta funcion no checa las piezas, recibe una pieza y la elimina de la barra (cual barra??)
	public void checkPiecesToPosisionate(GameObject pieceSelected)
	{
		piecesAvailable--;
		int toDelete = 0;
		for(int i=0; i<piecesInBar.Count; i++)
		{
			if(pieceSelected.name == piecesInBar[i].name)
			{
				toDelete=i;
				break;
			}
		}
		piecesInBar.RemoveAt (toDelete);

		//TODO: GameManager va llamaar initializeAvailablePieces cuando lo necesite
		if(piecesAvailable == 0)
		{
			initializeAvailablePieces();
		}
	}

	public void setRotatePiece(GameObject go)
	{
		if (go.GetComponent<Piece> ()) 
		{
			setRotatePiece(go.GetComponent<Piece>());
		}
	}

	//Se cambian las piezas por las rotadas
	public void setRotatePiece(Piece piece)
	{
		if (isRotating)
		{
			return;
		}

		if (piece.rotateTimes > 0) {
			isRotating = true;
			OnRotate (isRotating);

			//CHANGE: Los nombres deben ser lo mas claros y simples posibles, para contar las rotaciones pudo ser un rotateCount
			piece.howManyHasBeenRotated += 1;
				
			piece.gameObject.transform.DOScale (new Vector3 (0, 0), 0.25f).OnComplete (() => {

				piece.transform.localRotation = Quaternion.Euler(new Vector3(0,0,piece.transform.rotation.eulerAngles.z+90));
			
				piece.transform.DOScale (initialPieceScale, 0.25f).OnComplete (() => {
					isRotating = false;
					OnRotate (isRotating);
				});
			});

			if (piece.howManyHasBeenRotated > piece.rotateTimes) {
				piece.howManyHasBeenRotated = 0;
			}

			checkIfExistRotatedPiezes ();
		}
	}

	//regresamos la rotacion inicial si no se concreto la rotacion
	public void returnRotatePiecesToNormalRotation()
	{
		if (isRotating)
		{
			return;
		}

		for (int i = 0; i < piecesInBar.Count; i++) 
		{
			if (piecesInBar[i].howManyHasBeenRotated != 0) 
			{				
				int temp = piecesInBar[i].howManyHasBeenRotated;

				piecesInBar [i].transform.DOScale (new Vector3 (0.1f, 0.1f), 0.25f);
				StartCoroutine(pos(i,temp));
			}
		}
		checkIfExistRotatedPiezes ();
	}

	IEnumerator pos(int i,int rotateTimes)
	{
		yield return new WaitForSeconds (0.25f);
		piecesInBar [i].transform.localRotation = Quaternion.Euler(new Vector3(0,0,piecesInBar [i].transform.rotation.eulerAngles.z-(90 * rotateTimes)));
		piecesInBar [i].transform.DOScale (initialPieceScale, 0.25f);
	}

	//FIXME: Nombre mal escrito
	public bool checkIfExistRotatedPiezes()
	{
		for (int i = 0; i < piecesStock.childCount; i++) 
		{
			if (piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated != 0) 
			{	
				return true;
			}
		}
			
		return false;
	}

	public bool setRotationPiecesAsNormalRotation()
	{
		bool OneWasRotated = false;
		for (int i = 0; i < piecesStock.childCount; i++) 
		{
			if (piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated != 0) 
			{
				piecesStock.GetChild (i).GetComponent<Piece> ().howManyHasBeenRotated = 0;
				OneWasRotated = true;
			}
		}
		return OneWasRotated;
	}

	public void activateRotation(bool activate)
	{
		if(activate)
		{
			//Activar las imagenes de rotar
			for (int i = 0; i < piecesInBar.Count; i++) 
			{
				//firstPos [piecesInBar[i].GetComponent<Piece>().myFirstPosInt].GetComponent<Image> ().enabled = true;
			}
		}
		else
		{
			for (int i = 0; i < firstPos.Length; i++) 
			{
				firstPos [i].GetComponent<Image> ().enabled = false;
			}
			returnRotatePiecesToNormalRotation ();
		}
	}
}