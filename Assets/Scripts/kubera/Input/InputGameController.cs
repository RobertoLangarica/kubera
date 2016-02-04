using UnityEngine;
using System.Collections;
using DG.Tweening;
using ABC;

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
	protected PieceManager pieceManager;

	public CellsManager cellManager;

	protected GameObject piece;

	protected bool isLeterOfPice;
	protected bool isLetterSelected;
	protected bool isPiece;


	public float movingSpeed = .5f;
	public float movingUpFinger = 1.5f;

	protected Vector3 selectedScale = new Vector3 (4.5f, 4.5f, 4.5f);

	[HideInInspector]
	protected bool canRotate;
	protected bool destroyByColor;

	public delegate void rotateState (bool rotate);
	public rotateState deactivateRotateMode;

	public delegate void destroyByColorState (bool destroyByColor);
	public rotateState deactivateDestroyMode;

	public delegate void pieceSetCorrectly (int sizeOfPiece);
	public pieceSetCorrectly pointsAtPieceSetCorrectly;

	void Start () 
	{
		wordManager = GameObject.FindObjectOfType<WordManager>();
		pieceManager = GameObject.FindObjectOfType<PieceManager>();

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
				if (!piece) {return;}

				if(isLetterSelected)
				{
					wordManager.setPositionToLetters();
					wordManager.canSwappLetters(true,piece);
				}

				if (isPiece) 
				{
					Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					tempV3.z = -1;
					hasMoved = false;

					tempV3.y += movingUpFinger;
					//piece.transform.position = tempV3;
					piece.transform.DOMove(tempV3,.1f);
					piece.transform.DOScale(selectedScale,.1f);
				}
			}	
			break;
			
			case (ContinuousGesturePhase.Updated):
			{
				if(!isMultiFinger)
				{
					if (!piece) {return;}

					// si existe la pieza la movemos con movingLerping de acuerdo a la posicion del mouse
					if(isPiece)
					{
						Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
						tempV3.z = -1;
						hasMoved = true;
						tempV3.y += movingUpFinger;
						movingLerping(tempV3,piece);
					}
					else if(isLetterSelected)
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
			
				if(isLetterSelected)
				{
					//Lo habilitamos y ajustamos
			
					swappingLetter();
					piece = null;
					break;
				}
			
				//checamos que podamos poner la pieza
				if(!cellManager.CanPositionate(piece.GetComponent<Piece>().pieces))
				{
					if(destroyByColor)
					{
						Cell tempCell = cellManager.getCellOnVec(piece.transform.position);
						if(tempCell != null)
						{
							if(tempCell.occupied)
							{
								Debug.Log(tempCell.typeOfPiece);
								if(tempCell.typeOfPiece != ETYPEOFPIECE_ID.LETTER && tempCell.typeOfPiece != ETYPEOFPIECE_ID.LETTER_FROM_BEGINING)
								{	
									Debug.Log("Entro!!!!!!!!!!!!!");
									cellManager.selectCellsOfColor(tempCell);
										
									DestroyImmediate(piece);
								
									cellManager.turnSelectedCellsToLetters();
								}
								else
								{
									backToNormal();
								}
								destroyByColor = false;
								deactivateDestroyMode (destroyByColor);
							}
						}

						else
						{
							backToNormal();
							destroyByColor = false;
							deactivateDestroyMode (destroyByColor);
						}
					}
					else
					{
						backToNormal();
					}
				}

				else
				{
					if(destroyByColor)
					{
						backToNormal();
						destroyByColor = false;
						deactivateDestroyMode (destroyByColor);
					}
					else
					{

						//ponemos la pieza en su posicion correcta de manera suave y le quitamos el colider a la pieza completa
						Vector3 myNewPosition = cellManager.Positionate(piece.GetComponent<Piece>());
						DOTween.KillAll();
						piece.transform.DOMove(new Vector3(myNewPosition.x,myNewPosition.y,1),.1f);
						piece.GetComponent<BoxCollider2D>().enabled = false;

						//Ponemos los puntos de acuerdo a la cantidad de piezas

						pointsAtPieceSetCorrectly (piece.GetComponent<Piece> ().pieces.Length);

						afterDragEnded();
						cellManager.LineCreated();


						FlashColor[] flash = piece.GetComponentsInChildren<FlashColor>();
						foreach(FlashColor f in flash)
						{
							f.startFlash(f.GetComponent<SpriteRenderer>(),0.2f);
						}
					}

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
			checkWhatIs (gesture.Raycast.Hit2D.transform.gameObject);

			if(isPiece)
			{
				piece = gesture.Raycast.Hit2D.transform.gameObject;

				if(!canRotate)
				{
					Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					tempV3.z = -1;
					hasMoved = false;

					tempV3.y += movingUpFinger;
					//piece.transform.position = tempV3;
					piece.transform.DOMove(tempV3,.1f);
					piece.transform.DOScale(selectedScale,.1f);
				}

			}
			else if(isLetterSelected)
			{
				piece = gesture.Raycast.Hit2D.transform.gameObject;
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
				if (canRotate) 
				{
					choseToRotate(gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Piece>());
					return;
				}
				if(isLeterOfPice)
				{
					if(!gesture.Raycast.Hit2D.transform.gameObject.GetComponent<ABCChar>().isSelected)
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
		if(piece&&!hasMoved && !isLeterOfPice && !isLetterSelected)
		{
			backToNormal();
		}
		pieceManager.returnRotatePiecesToNormalRotation ();
	}

	// si la pieza no fue puesta correctamente la regresamos a su posicion inicial
	void backToNormal()
	{
		GameObject gotemp = piece.gameObject;

		/*
		 * checamos si es powerUp, la regresamos a su posicion y la destruimos al llegar ahi
		 * si no es powerUp la regresamos a su posicion inicial
		 */
		if(piece.GetComponent<Piece>().powerUp)
		{
			print(piece.GetComponent<Piece>().myFirstPos.position);
			Vector3 tempV3 = piece.GetComponent<Piece>().myFirstPos.position;
			piece.transform.DOMove (new Vector3 (tempV3.x, tempV3.y, 1), .2f).OnComplete(()=>{DestroyImmediate(gotemp);});
			piece.transform.DOScale (new Vector3 (0, 0, 0), .2f);
		}
		else
		{
			Vector3 tempV3 = piece.GetComponent<Piece>().myFirstPos.position;
			piece.transform.DOMove (new Vector3 (tempV3.x, tempV3.y, 1), .2f);


			piece.transform.DOScale (new Vector3 (2.5f,2.5f,2.5f), .1f);
						
			piece = null;
		}

		piece = null;
		isLeterOfPice = false;
		isLetterSelected = false;
		isPiece = false;
	}

	//hacemos que la pieza nos siga de manera mas suave
	public void movingLerping(Vector3 end,GameObject piece)
	{
		if (hasMoved) 
		{
			if(isLetterSelected)
			{
				piece.transform.DOMove (end, .8f).SetId("MovingPiece");
			}
			else
			{
				piece.transform.DOMove (end, .3f).SetId("MovingPiece");
			}
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

		checkWhatIs (go);
	}

	void OnSwipe(SwipeGesture gesture) 
	{
		if (isLetterSelected) 
		{
			if(gesture.Direction == FingerGestures.SwipeDirection.Up)
			{
				
			}
		}
	}

	protected void choseToRotate(Piece piece)
	{
		pieceManager.setRotatePieces(piece);
	}

	/*
	 * Cuando acaba el drag 
	 * checamos si es powerUp
	 */ 
	protected void afterDragEnded()
	{
		if(!piece.GetComponent<Piece>().powerUp)
		{
			if (canRotate) 
			{
				//utilizo el powerUp de rotar
				GameObject.Find ("PowerRotate").GetComponent<PowerUpBase> ().PowerUsed ();
				if (pieceManager.setRotationPiecesAsNormalRotation ()) 
				{
					//ChargePower
					print("ChargePowerUP");
				}
				canRotate = false;
				deactivateRotateMode (canRotate);

			}

			pieceManager.checkBarr(piece);
			piece.transform.SetParent (null);
		}
		else
		{
			GameObject.Find(piece.name).GetComponent<PowerUpBase>().PowerUsed();
		}
	}

	protected void swappingLetter()
	{
		DOTween.KillAll();

		wordManager.canSwappLetters(false,piece);
	}

	public void setCanRotate(bool rotate)
	{
		if (rotate) 
		{
			canRotate = true;
		}
		else 
		{
			canRotate = false;
		}
	}

	public void setDestroyByColor(bool activate)
	{
		if (activate) 
		{
			destroyByColor = true;
		}
		else 
		{
			destroyByColor = false;
		}
	}

	protected void checkWhatIs(GameObject go)
	{
		if (go.GetComponent<Piece> ()) 
		{
			isPiece = true;
			isLeterOfPice = false;
			isLetterSelected = false;
			return;
		}

		if (go.GetComponent<UIChar> ()) 
		{
			isPiece = false;
			if (go.GetComponent<ABCChar> ().isSelected) 
			{
				isLetterSelected = true;
			} 
			else 
			{
				isLeterOfPice = true;
			}
			return;
		}
		isPiece = false;
		isLeterOfPice = false;
		isLetterSelected = false;
	}
}
