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

	public CellsManager cellManager;
	protected GameObject piece;
	
	// Use this for initialization
	void Start () {
	
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


			Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
			tempV3.z = 0;
			//piece.transform.position = tempV3;
			if(gesture.Raycast.Hits2D != null)
			{
				//if(gesture.Raycast.Hit2D.transform.gameObject.GetComponents

				if(gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Piece>())
				{
					piece = gesture.Raycast.Hit2D.transform.gameObject;
				}

			}
		}	
			break;
			
		case (ContinuousGesturePhase.Updated):
		{
			if(!isMultiFinger)
			{
				if(piece)
				{
					Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					tempV3.z = -1;
					hasMoved = true;
					tempV3.y += 1.25f;
					movingLerping(tempV3,piece);
					//piece.transform.position = tempV3;
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


			//checar si la pieza fue puesta correctamente
			if(!piece)
			{
				break;
			}


			if(!cellManager.CanPositionate(piece.GetComponent<Piece>().pieces))
			{
				backToNormal();
			}
			else
			{
				Vector3 myNewPosition = cellManager.Positionate(piece.GetComponent<Piece>());
				piece.transform.DOMove(new Vector3(myNewPosition.x,myNewPosition.y,1),.1f);
			
				piece.GetComponent<BoxCollider2D>().enabled = false;
				//checamos si es powerup para no rellenar la barra
				if(!piece.GetComponent<Piece>().powerUp)
				{
					PieceManager.instance.checkBarr(piece);
				}
				else
				{
					FindObjectOfType<PowerUpSpaces>().powerUses--;
				}

				cellManager.LineCreated();

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
				
				if(piece)
				{
					Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
					tempV3.z = -1;
					hasMoved = false;

					tempV3.y += 1.25f;
					//piece.transform.position = tempV3;
					piece.transform.DOMove(tempV3,.1f);
					piece.transform.DOScale(new Vector3(4.5f,4.5f,4.5f),.1f);
				}
			}
		}
	}

	void OnTap(TapGesture gesture)
	{
		if(gesture.Raycast.Hits2D != null)
		{
			if(gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Tile>())
			{
				gesture.Raycast.Hit2D.transform.gameObject.GetComponent<Tile>().ShootLetter();
			}
		}
	}

	void OnFingerUp()
	{
		if(piece&&!hasMoved)
		{
			backToNormal();
		}
	}

	void backToNormal()
	{

		Vector3 tempV3 = piece.GetComponent<Piece> ().myFirstPos.position;
		piece.transform.DOMove (new Vector3 (tempV3.x, tempV3.y, 1), .2f);
		piece.transform.DOScale (new Vector3 (4, 4, 4), .1f);
	}

	public void movingLerping(Vector3 end,GameObject piece)
	{
		if (hasMoved) 
		{
			piece.transform.DOMove (end, .3f).SetId("MovingPiece");
		}
	}

	public void checkToLoose()
	{
		if(!cellManager.VerifyPosibility(PieceManager.instance.piecesInBar) && FindObjectOfType<PowerUpSpaces>().powerUses ==0)
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
			FindObjectOfType<WordManager>().checkIfAWordisPossible(PieceManager.instance.listChar);
		}
	}
}
