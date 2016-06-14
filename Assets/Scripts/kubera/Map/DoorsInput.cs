﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class DoorsInput : MonoBehaviour {

	public ScrollRect scrollRect;
	public bool canMove;
	public bool moved;
	public RectTransform finalPositionLeftDoor;
	public RectTransform finalPositionRightDoor;
	public BoxCollider2D boxCollider2DLeftDoor;
	public BoxCollider2D boxCollider2DRightDoor;

	public Image finalLeftDoor;
	public Image finalRightDoor;

	public RectTransform rectTransformLeft;
	public RectTransform rectTransformRight;
	public GameObject leftDoor;
	public GameObject rightDoor;

	public float oppenDoorSpeed = 1f;

	void Start()
	{
		StartCoroutine (setBoxColliderSize());

		if(canMove)
		{
			DestroyDoors ();
		}
	}

	IEnumerator setBoxColliderSize()
	{
		yield return new WaitForSeconds (0.1f);
		boxCollider2DLeftDoor.size = rectTransformLeft.rect.size;
		boxCollider2DRightDoor.size = rectTransformRight.rect.size;
	}

	protected void OnSwipe(SwipeGesture gesture)
	{
		if(canMove)
		{
			if((FingerGestures.SwipeDirection.Right == gesture.Direction || FingerGestures.SwipeDirection.Left == gesture.Direction) && !moved)
			{
				moved = false;
				MoveDoor (rectTransformLeft, finalPositionLeftDoor.localPosition, oppenDoorSpeed, finalLeftDoor,leftDoor);
				MoveDoor (rectTransformRight, finalPositionRightDoor.localPosition, oppenDoorSpeed, finalRightDoor,rightDoor);
			}
		}
	}

	void OnFingerDown(FingerDownEvent  gesture)
	{
		if(canMove && gesture.Raycast.Hits2D != null)
		{
			scrollRect.enabled = false;
		}
	}

	void OnFingerUp()
	{
		scrollRect.enabled = true;
	}

	protected void MoveDoor(RectTransform rectTransform, Vector3 finalPosition,float speed,Image finalDoor,GameObject doorToDestroy)
	{
		//rectTransform.DOLocalMove (finalPosition, speed).OnComplete(()=>{openDoor(finalDoor);Destroy (doorToDestroy); });
		rectTransform.DOAnchorPos(Vector2.zero,speed).OnComplete(()=>{openDoor(finalDoor);});
	}

	protected void openDoor(Image finalDoor,float speed = 0.5f)
	{
		//finalDoor.DOFade(1,speed);
	}

	protected void DestroyDoors()
	{
		Destroy (leftDoor);
		Destroy (rightDoor);
	}
}
