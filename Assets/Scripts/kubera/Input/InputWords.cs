using UnityEngine;
using System.Collections;
using DG.Tweening;
using ABC;

public class InputWords : MonoBehaviour 
{
	protected int lastTimeDraggedFrame;
	protected GameObject letter;
	protected GameObject target;

	//Para notificar estados del drag a otros objetos
	public delegate void DInputWordNotification(GameObject letter);

	public DInputWordNotification onDragFinish;
	public DInputWordNotification onDragUpdate;
	public DInputWordNotification onDragStart;
	public DInputWordNotification onTap;
	public DInputWordNotification onTapToDelete;

	public float letterSpeed = 0.5f;
	public bool allowInput = true;
	protected bool canDeleteLetter = true;
	protected bool drag = false;
	public Vector3 offsetPositionOverFinger = new Vector3(0,0.5f,0);
	float offset = 0;

	//Foa animation on grid
	public float scalePercent = 0.8f;
	public float animationTime = 0.5f;
	protected bool allowPushDownAnimation = true;
	protected bool allowAnimation = true;

	protected Vector3 firstPosition;

	protected Vector2 objectSize;

	void Start()
	{
		onDragFinish += foo;
		onDragUpdate += foo;
		onDragStart += foo;
		onTap += foo;
		onTapToDelete += foo;

	}
	public void foo(GameObject go){}


	void OnDrag(DragGesture gesture) 
	{
		//Solo se ejecuta una vez por frame (para que el multifinger funcione sin encimarse)
		if(!allowInput || lastTimeDraggedFrame == Time.frameCount)
		{
			return;
		}

		lastTimeDraggedFrame = Time.frameCount;

		switch(gesture.Phase)
		{
		case (ContinuousGesturePhase.Started):
			{	
				if (!gesture.Raycast.Hit2D) 
				{
					if(!allowAnimation)
					{		
						animationFingerUp ();
					}
					return;
				}
				offset = letter.transform.position.y;

				canDeleteLetter = false;
				//letter = gesture.Raycast.Hit2D.transform.gameObject;
				offset += objectSize.y;

				onDragStart(letter);
				drag = true;
			}	
			break;

		case (ContinuousGesturePhase.Updated):
			{
				if (!letter) 
				{return;}
					
				Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));

				tempV3.y = offset;
				tempV3.z = letter.transform.position.z;
				moveTo(letter,tempV3,letterSpeed);

				onDragUpdate (letter);							
			}
			break;

		case (ContinuousGesturePhase.Ended):
			{	
				if (!letter) 
				{
					return;
				}
				//letter.transform.position = firstPosition;
				//onDragFinish(letter);
				//DOTween.Kill ("InputW_Dragging");

				//letter.transform.position = new Vector3 (letter.transform.position.x, 0, 0);	

				//letter = null;

				//canDeleteLetter = true;

			}
			break;
		}
	}

	void OnFingerDown(FingerDownEvent gesture)
	{
		if (allowInput && gesture.Raycast.Hit2D) 
		{
			letter = gesture.Raycast.Hit2D.transform.gameObject;
			firstPosition = letter.transform.position;

			if(objectSize.x == 0 && gesture.Raycast.GameObject.GetComponent<BoxCollider2D>())
			{				
				objectSize = gesture.Raycast.GameObject.GetComponent<BoxCollider2D> ().bounds.size;
				print (objectSize);
			}
		}
	}

	void OnLongPress(LongPressGesture gesture)
	{
		if(allowInput && gesture.Raycast.Hit2D)
		{
			Vector3 tempV3 = new Vector3 ();

			offset = letter.transform.position.y;
			offset += objectSize.y;

			tempV3.x = letter.transform.position.x;
			tempV3.y = offset;//offset;
			tempV3.z = letter.transform.position.z;

			letter.transform.position = tempV3;

			//moveTo(letter,tempV3,letterSpeed);
			canDeleteLetter = false;
			onDragStart(letter);

		}
	}

	void OnFingerUp(FingerUpEvent gesture)
	{
		if (allowInput && canDeleteLetter == false && letter) 
		{
			onDragFinish(letter);

			letter.transform.position = new Vector3(letter.transform.position.x,firstPosition.y,0);
			DOTween.Kill ("InputW_Dragging");
			letter = null;
		}
		canDeleteLetter = true;
		drag = false;
	}

	void OnLetterGridFingerDown(FingerDownEvent gesture)
	{
		if (allowInput && gesture.Raycast.Hit2D && allowPushDownAnimation) 
		{
			target = gesture.Raycast.Hit2D.transform.gameObject;
			Vector3 finalScale = target.transform.localScale;
			target.transform.localScale = finalScale * scalePercent;
			allowPushDownAnimation = false;
			allowAnimation = false;
		}
	}

	void OnLetterGridFingerUp(FingerUpEvent gesture)
	{
		if (allowInput && target != null) 
		{

			if(!allowAnimation)
			{		
				animationFingerUp ();
			}
		}
	}

	void animationFingerUp()
	{
		Vector3 finalScale = target.transform.localScale / scalePercent;

		allowAnimation = true;

		DOTween.Kill ("InputW_Grid_Scale_Selection");
		target.transform.DOScale (finalScale, animationTime).OnComplete(()=>
			{
				allowPushDownAnimation = true;
				target = null;
			});
	}

	void OnLetterGridTap(TapGesture gesture)
	{
		if(allowInput && gesture.Raycast.Hit2D && allowAnimation)
		{		
			GameObject target = gesture.Raycast.Hit2D.transform.gameObject;

			if (target.layer == LayerMask.NameToLayer ("LetterOnGrid")) 
				{
					onTap(gesture.Raycast.Hit2D.transform.gameObject);
				}	
		}
	}



	void OnLetterWordTap(TapGesture gesture)
	{
		if (allowInput && canDeleteLetter && gesture.Raycast.Hit2D) 
		{	
			onTapToDelete(gesture.Raycast.Hit2D.transform.gameObject);
		}
	}

	public void moveTo(GameObject target, Vector3 to, float delay = 0.1f)
	{
		DOTween.Kill("InputW_Dragging",false);
		target.transform.DOMove (to, delay).SetId("InputW_Dragging");
	}

}