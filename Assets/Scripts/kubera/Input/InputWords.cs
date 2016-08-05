using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using ABC;

public class InputWords : MonoBehaviour 
{
	protected int lastTimeDraggedFrame;
	public GameObject letter;
	public GameObject gridLetter;

	public RectTransform wordsContainer;

	//Para notificar estados del drag a otros objetos
	public delegate void DInputWordNotification(GameObject letter);

	public DInputWordNotification onDragStart;

	public delegate void DInputLetterNotifyChange(GameObject letter,bool correctlyOnContainer);

	public DInputLetterNotifyChange onDragUpdate;
	public DInputLetterNotifyChange onDragFinish;
	public DInputLetterNotifyChange onChangePutLetterOverContainer;

	public delegate void DInputWordNotificationLetterOnGrid(GameObject letter,bool byDrag=false);

	public DInputWordNotificationLetterOnGrid onTap;
	public DInputWordNotificationLetterOnGrid onTapToDelete;
	public DInputWordNotification onLetterOnGridDragFinish;

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
	public bool allowAnimation = true;
	protected Vector3 firstPosition;
	protected Vector3 pushedScale = new Vector3(0.8f,0.8f,0.8f);
	protected Vector3 normalScale = new Vector3(1.0f,1.0f,1.0f);

	protected Vector2 objectSize;
	protected bool isOnLettersContainer;

	public float limitWidth;

	void Start()
	{
		onDragFinish += foo;
		onDragUpdate += foo;
		onDragStart += foo;
		onTap += foo;
		onTapToDelete += foo;
		onLetterOnGridDragFinish += foo;
		onChangePutLetterOverContainer += foo;

	}
	public void foo(GameObject go,bool byDrag=false){}


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
				if(gridLetter != null)
				{
					onTap(gridLetter,true);
				}
				if(letter == null)
				{
					return;
				}

				if (!gesture.Raycast.Hit2D && gridLetter) 
				{
					if(!allowAnimation)
					{		
						animationFingerUp (gridLetter);
					}
				}

				canDeleteLetter = false;
				//letter = gesture.Raycast.Hit2D.transform.gameObject;
				offset += objectSize.y*0.5f;

				onDragStart(letter);

				drag = true;

				onChangePutLetterOverContainer (letter, !gridLetter);
				isOnLettersContainer = !gridLetter;

				if(objectSize.x == 0 && letter.GetComponent<BoxCollider2D>())
				{				
					letter.GetComponent<BoxCollider2D> ().enabled = true;

					objectSize = letter.GetComponent<BoxCollider2D> ().bounds.size;
				}
			}	
			break;

		case (ContinuousGesturePhase.Updated):
			{
				if (!letter) 
				{return;}
				Vector3 tempV3 = Camera.main.ScreenToWorldPoint(new Vector3(gesture.Position.x,gesture.Position.y,0));



				tempV3.y += objectSize.y*2f;
				tempV3.z = letter.transform.position.z;

				//limite de zona de letras
				/*if(gesture.Position.x > limitWidth || gesture.Position.x < 33)
				{
					//tempV3.x = letter.transform.position.x-0.01f;
					//return;
				}*/

				if(isLetterGridPositionatedCorrectly ())
				{
					if(!isOnLettersContainer)
					{
						isOnLettersContainer = true;
						onChangePutLetterOverContainer (letter, true);
					}
				}
				else
				{
					if(isOnLettersContainer)
					{
						isOnLettersContainer = false;
						onChangePutLetterOverContainer (letter, false);
					}
				}


				moveTo(letter,tempV3,letterSpeed);
				onDragUpdate (letter,isOnLettersContainer);	
				//print (letter.GetComponent<RectTransform>().anchoredPosition.x);
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
				//
				//letter.transform.position = new Vector3 (letter.transform.position.x, 0, 0);	
				//
				//letter = null;
				//
				//canDeleteLetter = true;

			}
			break;
		}
	}

	//Letters on Word
	void OnFingerDown(FingerDownEvent gesture)
	{
		if (allowInput && gesture.Raycast.Hit2D) 
		{
			letter = gesture.Raycast.Hit2D.transform.gameObject;
			firstPosition = letter.transform.position;

			if(objectSize.x == 0 && gesture.Raycast.GameObject.GetComponent<BoxCollider2D>())
			{				
				objectSize = gesture.Raycast.GameObject.GetComponent<BoxCollider2D> ().bounds.size;
			}
		}
	}

	void OnLongPress(LongPressGesture gesture)
	{
		if(allowInput && gesture.Raycast.Hit2D)
		{
			Vector3 tempV3 = new Vector3 ();

			offset = letter.transform.position.y;
			offset += objectSize.y*0.5f;

			tempV3.x = letter.transform.position.x;
			tempV3.y = offset;//offset;
			tempV3.z = letter.transform.position.z;

			//letter.transform.position = tempV3;

			moveTo(letter,tempV3,letterSpeed);
			canDeleteLetter = false;
			onDragStart(letter);

		}
	}

	void OnFingerUp(FingerUpEvent gesture)
	{
		if (allowInput && letter) 
		{
			
			onDragFinish(letter,isOnLettersContainer);

			letter.transform.position = new Vector3(letter.transform.position.x,firstPosition.y,0);
			DOTween.Kill ("InputW_Dragging");

			if(!isOnLettersContainer)
			{
				if(gridLetter)
				{					
					onTap(gridLetter);
				}
				else
				{
					if(!canDeleteLetter)
					{
						print ("S");
					}
					else
					{
						onTapToDelete (letter);
					}
				}
			}
			gridLetter = null;
			letter = null;
		}
		canDeleteLetter = true;
		drag = false;
	}

	bool isLetterGridPositionatedCorrectly()
	{
		Vector3 v3;

		v3 = letter.transform.position;
		v3 = Camera.main.WorldToScreenPoint (v3);

		v3.x = v3.x/Screen.width;

		/*print (v3);
		print (wordsContainer.anchorMin);
		print (wordsContainer.anchorMax);*/
		if (v3.x > wordsContainer.anchorMin.x && v3.x < wordsContainer.anchorMax.x) 
		{
			v3.y = v3.y/Screen.height;
			if (v3.y > wordsContainer.anchorMin.y && v3.y < wordsContainer.anchorMax.y) 
			{
				return true;
				//onLetterOnGridDragFinish (letter);
			}
			else
			{
				/*animationFingerUp (target);
				onTap(target);*/
				return false;

			}
		}
		return false;
	}

	void OnLetterGridFingerDown(FingerDownEvent gesture)
	{
		if (allowInput && gesture.Raycast.Hit2D && allowPushDownAnimation) 
		{
			gridLetter = gesture.Raycast.Hit2D.transform.gameObject;

			gridLetter.transform.localScale = pushedScale;

			allowPushDownAnimation = true;
			allowAnimation = false;
		}
	}

	void OnLetterGridFingerUp(FingerUpEvent gesture)
	{
		if (allowInput && gridLetter != null) 
		{
			if(!allowAnimation)
			{		
				animationFingerUp (gridLetter);
			}
		}
	}

	void animationFingerUp(GameObject go)
	{
		allowAnimation = true;

		allowPushDownAnimation = true;
		gridLetter.transform.localScale = normalScale;

		//DOTween.Kill ("InputW_Grid_Scale_Selection");
		//go.transform.DOScale (normalScale, animationTime).SetId ("InputW_Grid_Scale_Selection");
	}

	void OnLetterGridTap(TapGesture gesture)
	{
		if(allowInput && gridLetter && allowAnimation)
		{		
			if (gridLetter.layer == LayerMask.NameToLayer ("LetterOnGrid")) 
			{
				onTap(gridLetter);
				gridLetter = null;
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