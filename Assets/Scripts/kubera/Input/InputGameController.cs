using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InputGameController : MonoBehaviour {

	protected bool hasMoved;

	//Para controlar cuando el gesto viene de mas de un dedo
	protected bool isDragging = false;
	protected bool isMultiFinger = false;
	protected int lastDragFrame;
	protected int fingerCount = 0;
	
	//Para notificar estados del drag a otros objetos
	public delegate void DOnDragFinish();
	public delegate void DOnDrag();
	public delegate void DOnDragStart();
	[HideInInspector]
	public DOnDragFinish onDragFinish;
	[HideInInspector]
	public DOnDrag onAnyDrag;
	[HideInInspector]
	public DOnDragStart onDragStart;

	protected WordManager wordManager;
	protected GameManager gameManager;
	public CellsManager cellManager;
	protected GameObject piece;
	protected GameObject pieceReference;
	protected bool isLeter;
	public float movingSpeed = .5f;
	public float movingUpFinger = 1.5f;

	void Start () 
	{
		gameManager = GameObject.FindObjectOfType<GameManager>();
		wordManager = GameObject.FindObjectOfType<WordManager>();

		onDragFinish = foo;
		onAnyDrag	 = foo;
		onDragStart	 = foo;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	protected void foo(){}
	
	void OnDrag(DragGesture gesture) 
	{
		//Solo se ejecuta una vez por frame (para que el multifinger funcione sin encimarse)
		if(lastDragFrame == Time.frameCount)
		{
			return;
		}
		
		lastDragFrame = Time.frameCount;
		
		onAnyDrag();
		
		switch(gesture.Phase)
		{
		case (ContinuousGesturePhase.Started):
		{
			isDragging = true;
			hasMoved = true;
			onDragStart();


			if(gesture.Raycast.Hits2D != null)
			{
				//if(gesture.Raycast.Hit2D.transform.gameObject.GetComponents

				if(gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Piece>() && !gameManager.canRotate)
				{
					piece = gesture.Raycast.Hit2D.transform.gameObject;
					isLeter = false;
				}

				if(gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Letter>())
				{
					piece = gesture.Raycast.Hit2D.transform.gameObject;
					isLeter = true;
					wordManager.setPositionToLetters();
					wordManager.canSwappLetters(true,piece);
				}
			}
		}	
			break;
			
		case (ContinuousGesturePhase.Updated):
		{
			if(!isMultiFinger)
			{
				// si existe la pieza la movemos con movingLerping de acuerdo a la posicion del mouse
				if(piece && !isLeter)
				{
					Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					tempV3.z = -1;
					hasMoved = true;
					tempV3.y += movingUpFinger;
					movingLerping(tempV3,piece);
				}
				else if(piece && isLeter)
				{
					float y = piece.transform.position.y;
					float z = piece.transform.position.z;
					Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					tempV3.z = -1;
					hasMoved = true;
					//tempV3.y += movingUpFinger;
					tempV3.y = y;
					tempV3.z = z;
					wordManager.swappingLetters(piece);
					movingLerping(tempV3,piece);
				}
			}
			else
			{}
		}
			break;
		case (ContinuousGesturePhase.Ended):
		{
			isDragging = false;
			hasMoved = false;
			onDragFinish();


			//si no hay pieza terminamos
			if(!piece)
			{
				break;
			}

			if(piece.GetComponent<Letter>())
			{
				//Lo habilitamos y ajustamos

				swappingLetter();
				piece = null;
				break;
			}

			//checamos que podamos poner la pieza
			if(!cellManager.CanPositionate(piece.GetComponent<Piece>().pieces))
			{
				backToNormal();
			}
			else
			{
				//ponemos la pieza en su posicion correcta de manera suave y le quitamos el colider a la pieza completa
				Vector3 myNewPosition = cellManager.Positionate(piece.GetComponent<Piece>());
				DOTween.KillAll();
				piece.transform.DOMove(new Vector3(myNewPosition.x,myNewPosition.y,1),.1f);
				piece.GetComponent<BoxCollider2D>().enabled = false;

				//Ponemos los puntos de acuerdo a la cantidad de piezas
				gameManager.addPoints(piece.GetComponent<Piece>().pieces.Length);


				afterDragEnded();

				cellManager.LineCreated();

				//if(!cellManager.LineCreated())
				//{
					FlashColor[] flash = piece.GetComponentsInChildren<FlashColor>();

					foreach(FlashColor f in flash)
					{
						f.startFlash(f.GetComponent<SpriteRenderer>(),0.2f);
					}
				//}

				//Checamos si ya no puede hacer ningun movimiento
				checkToLoose();

			}
			DOTween.Kill("MovingPiece");
			piece = null;

		}
			break;
			
		}
	}


	void OnFingerDown(FingerDownEvent  gesture)
	{
		if(gesture.Raycast.Hits2D != null)
		{
			if(gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Piece>())
			{
				piece = gesture.Raycast.Hit2D.transform.gameObject;
				isLeter = false;
				if(piece)
				{
					if(gameManager.canRotate)
					{
						if(piece.GetComponent<Piece>().firstPiece&&piece != pieceReference)
						{
							choseToRotate(piece.GetComponent<Piece>());
							pieceReference = piece;
							piece = null;
						}
						else
						{
							Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
							tempV3.z = -1;
							hasMoved = false;
							
							tempV3.y += movingUpFinger;
							//piece.transform.position = tempV3;
							piece.transform.DOMove(tempV3,.1f);
							piece.transform.DOScale(new Vector3(4.5f,4.5f,4.5f),.1f);
						}
					}
					else
					{
						Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
						tempV3.z = -1;
						hasMoved = false;
						
						tempV3.y += movingUpFinger;
						//piece.transform.position = tempV3;
						piece.transform.DOMove(tempV3,.1f);
						piece.transform.DOScale(new Vector3(4.5f,4.5f,4.5f),.1f);
					}
				}
			}
			else if(gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Letter>())
			{
				//print("Letter");
			}
		}
	}

	void OnTap(TapGesture gesture)
	{
		if(gesture.Raycast.Hits2D != null)
		{
			if(gesture.Raycast.Hit2D.transform)
			{
				if(gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Tile>())
				{
					if(gameManager.destroyByColor)
					{
						gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Tile>().selectPieceColorToDestroy();
					}
					else
					{
						gesture.Raycast.Hit2D.transform.gameObject.GetComponent<ABCChar>().ShootLetter();
						FindObjectOfType<ShowNext>().ShowingNext(true);
						gameObject.GetComponent<AudioSource>().Play();
					}
				}
			}
		}
	}

	void OnFingerUp()
	{
		if(piece&&!hasMoved && !piece.GetComponent<Letter>())
		{
			backToNormal();
		}
	}

	// si la pieza no fue puesta correctamente la regresamos a su posicion inicial
	void backToNormal()
	{
		GameObject gop = piece.gameObject;

		/*
		 * checamos si es powerUp, la regresamos a su posicion y la destruimos al llegar ahi
		 * si no es powerUp la regresamos a su posicion inicial
		 */
		if(piece.GetComponent<Piece>().powerUp)
		{
			print(piece.GetComponent<Piece>().myFirstPos.position);
			Vector3 tempV3 = piece.GetComponent<Piece>().myFirstPos.position;
			piece.transform.DOMove (new Vector3 (tempV3.x, tempV3.y, 1), .2f).OnComplete(()=>{DestroyImmediate(gop);});
			piece.transform.DOScale (new Vector3 (0, 0, 0), .2f);

		}
		else
		{
			Vector3 tempV3 = piece.GetComponent<Piece>().myFirstPos.position;
			piece.transform.DOMove (new Vector3 (tempV3.x, tempV3.y, 1), .2f);
			if(piece.GetComponent<Piece>().firstPiece)
			{
				piece.transform.DOScale (new Vector3 (4, 4, 4), .1f);
			}
			else
			{
				piece.transform.DOScale (new Vector3 (1.5f, 1.5f, 1.5f), .1f);
			}
			
			piece = null;
		}
	}

	//hacemos que la pieza nos siga de manera mas suave
	public void movingLerping(Vector3 end,GameObject piece)
	{
		if (hasMoved) 
		{
			if(piece.GetComponent<Letter>())
			{
				piece.transform.DOMove (end, .8f).SetId("MovingPiece");
			}
			else
			{
				piece.transform.DOMove (end, .3f).SetId("MovingPiece");
			}
		}
	}

	IEnumerator check()
	{
		yield return new WaitForSeconds (.2f);
		FindObjectOfType<WordManager>().checkIfAWordisPossible(PieceManager.instance.listChar);
	}

	public void checkToLoose()
	{
		if(!cellManager.VerifyPosibility(PieceManager.instance.piecesInBar) && FindObjectOfType<PowerUpBase>().uses ==0)
		{
			Debug.Log ("Perdio");
			while(true)
			{
				bool pass = true;
				for(int i=0; i < PieceManager.instance.listChar.Count; i++)
				{
					if(!PieceManager.instance.listChar[i])
					{
						PieceManager.instance.listChar.RemoveAt(i);
						i--;
						pass = false;
					}
				}
				if(pass)
				{
					break;
				}
			}
			StartCoroutine(check());

		}
	}

	public void activePowerUp(GameObject go)
	{
		piece = go;

		Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0));
		tempV3.z = -1;
		hasMoved = false;
		
		tempV3.y += movingUpFinger;
		//piece.transform.position = tempV3;
		piece.transform.DOMove(tempV3,.1f);
		piece.transform.DOScale(new Vector3(4.5f,4.5f,4.5f),.1f);
	}

	void OnSwipe(SwipeGesture gesture) 
	{
		if (!piece) 
		{
			if(gesture.Direction == FingerGestures.SwipeDirection.Up)
			{
				FindObjectOfType<WordManager>().resetValidation();
				checkToLoose();
			}
		}
	}

	protected void choseToRotate(Piece piece)
	{
		PieceManager.instance.setRotatePieces(piece);
	}

	/*
	 * Cuando acaba el drag 
	 * checamos si es powerUp
	 */ 
	protected void afterDragEnded()
	{
		if(!piece.GetComponent<Piece>().powerUp)
		{
			if((piece.GetComponent<Piece>().firstPiece == false && pieceReference )||(piece.GetComponent<Piece>().firstPiece == true && pieceReference))
			{
				if(pieceReference != piece)
				{
					PieceManager.instance.destroyRotatePieces(piece.GetComponent<Piece>());
					//utilizo el powerUp de rotar
					GameObject.Find("PowerRotate").GetComponent<PowerUpBase>().PowerUsed();
				}
				else
				{
					PieceManager.instance.destroyRotatePieces(piece.GetComponent<Piece>(),false);

				}
				pieceReference = null;
				gameManager.canRotate = false;
			}
			

			PieceManager.instance.checkBarr(piece);
		}
		else
		{
			GameObject.Find(piece.name).GetComponent<PowerUpBase>().PowerUsed();
			//FindObjectOfType<PowerUpBase>().PowerUsed();
		}
	}

	protected void swappingLetter()
	{
		DOTween.KillAll();


		wordManager.canSwappLetters(false,piece);
	}
}
