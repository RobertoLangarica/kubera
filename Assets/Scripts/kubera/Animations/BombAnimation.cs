﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombAnimation : MonoBehaviour 
{
	public delegate void DAnimationNotification();
	public delegate void DOnAnimationFinish(Cell cell,Letter letter);
	
	public AnimatedSprite Animation;

	public DAnimationNotification OnAllAnimationsCompleted;
	public DOnAnimationFinish OnCellFlipped;

	protected CellsManager cellsManager;
	protected WordManager wordManager;

	protected List<AnimatedSprite> freeAnimation = new List<AnimatedSprite>();
	protected List<AnimatedSprite> occupiedAnimation = new List<AnimatedSprite>();

	protected int animationsFinished =0;
	protected int animationsCount =0;

	void Start () 
	{
		cellsManager = FindObjectOfType<CellsManager>();
		wordManager = FindObjectOfType<WordManager>();

		for(int i=0; i<1; i++)
		{
			addAnimationToThePool ();
		}
	}

	protected void addAnimationToThePool()
	{
		GameObject go;
		go = GameObject.Instantiate(Animation.gameObject) as GameObject;
		freeAnimation.Add(go.GetComponent<AnimatedSprite>());
		go.transform.SetParent(transform,false);
	}

	public IEnumerator startSinglePieceAnimation(Cell cellSelected)
	{
		Square square = cellSelected.content.GetComponent<Square> ();

		Letter letter = wordManager.getGridLetterFromPool (WordManager.EPoolType.NORMAL);
		letter.gameObject.SetActive(false);

		yield return new WaitForSeconds (0.1f);
		StartCoroutine (startAnimation (square, cellSelected, letter, 0.1f));

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("bombBlock");
			AudioManager.GetInstance().Play("bombBlock");
		}
	}

	public IEnumerator startSameColorSearchAnimation(Cell cellSelected)
	{
		Cell[] selection =  cellsManager.getCellNeighborsOfSameColor(cellSelected);
		List<Cell> selectionList = new List<Cell>();
		for(int i=0; i<selection.Length; i++)
		{
			selectionList.Add (selection [i]);
		}

		animationsFinished = 0;
		animationsCount = selectionList.Count;
		while (selectionList.Count >0)
		{
			int random = Random.Range (0, selectionList.Count);
			yield return new WaitForSeconds (0.1f);

			Square square = selectionList [random].content.GetComponent<Square> ();

			Letter letter = wordManager.getGridLetterFromPool (WordManager.EPoolType.NORMAL);
			letter.gameObject.SetActive(false);

			StartCoroutine (startAnimation (square, selectionList [random], letter, 0.1f));
			selectionList.Remove (selectionList [random]);

			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("bombBlock");
				AudioManager.GetInstance().Play("bombBlock");
			}
		}

	}

	IEnumerator startAnimation(Square square,Cell cellParent,Letter letter,float delay)
	{
		AnimatedSprite animSprite = getFreeAnimation ();
		letter.gameObject.SetActive(false);


		Vector3 cellPosition =  cellParent.transform.position + (new Vector3 (cellParent.GetComponent<SpriteRenderer> ().bounds.extents.x,
			-cellParent .GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

		animSprite.gameObject.transform.position = cellPosition;
		animSprite.gameObject.SetActive(true);

		yield return new WaitForSeconds (delay);

		animSprite.enabled = true;
		animSprite.autoUpdate = true;
		yield return new WaitUntil (()=> animSprite.sequences[0].currentFrame >= 6);
		square.OnCellFlipped += callbackOnFliped;
		square.doFlip (cellParent, letter, delay);

		yield return new WaitUntil (()=> animSprite.sequences[0].currentFrame >= 11);

		animSprite.enabled = false;
		animSprite.autoUpdate = false;
		animSprite.sequences [0].currentFrame = 0;
		releaseAnimation (animSprite);
	}

	public AnimatedSprite getFreeAnimation()
	{
		if(freeAnimation.Count == 0)
		{
			addAnimationToThePool();
		}

		AnimatedSprite anim = freeAnimation[0];
		freeAnimation.RemoveAt (0);

		occupiedAnimation.Add (anim);

		anim.gameObject.SetActive(true);

		return anim;
	}

	public void releaseAnimation(AnimatedSprite animation)
	{
		AnimatedSprite anim = occupiedAnimation[0];
		occupiedAnimation.RemoveAt (0);

		freeAnimation.Add (anim);

		anim.gameObject.SetActive(false);
	}



	protected void callbackOnFliped(Square square,Cell cell, Letter letter)
	{
		square.OnCellFlipped -= callbackOnFliped;
		letter.enabled = true;
		//cellsManager.occupyAndConfigureCell(cell,letter.gameObject,Piece.EType.LETTER,Piece.EColor.NONE,true);

		if (OnCellFlipped != null) 
		{
			OnCellFlipped (cell,letter);
		}
		animationsFinished ++;


		if (animationsFinished == animationsCount && OnAllAnimationsCompleted != null) 
		{
			OnAllAnimationsCompleted ();
		}


	}
}
