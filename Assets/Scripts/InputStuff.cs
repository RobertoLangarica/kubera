using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InputStuff : MonoBehaviour {

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
				piece = gesture.Raycast.Hit2D.transform.gameObject;

				print(piece.gameObject.GetComponent<BoxCollider2D>().offset);
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
					tempV3.z = 1;
					//hasMoved = false;
					tempV3.y += 1.5f;
					piece.transform.position = tempV3;
				}
			}
			else
			{}
		}
			break;
		case (ContinuousGesturePhase.Ended):
		{
			isDragging = false;
			//hasMoved = false;
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
				PieceManager.instance.checkBarr(piece);
				cellManager.LineCreated();
				if(!cellManager.VerifyPosibility(PieceManager.instance.piciesInBar))
				{
					Debug.Log ("Perdio");
				}
				else
				{
					Debug.Log ("Sigue");
				}
			}

			piece = null;
		}
			break;
			
		}
	}

	void OnFingerDown(FingerDownEvent  gesture)
	{
		if(gesture.Raycast.Hits2D != null)
		{
			piece = gesture.Raycast.Hit2D.transform.gameObject;
			
			if(piece)
			{
				Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));
				tempV3.z = 1;
				hasMoved = false;
				
				tempV3.y += 1.5f;
				piece.transform.DOMove(tempV3,.2f);
				piece.transform.DOScale(new Vector3(4.5f,4.5f,4.5f),.1f);
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
		piece.transform.DOMove(piece.GetComponent<Piece>().myFirstPos.position,.2f);
		piece.transform.DOScale(new Vector3(4,4,4),.1f);
	}
}
