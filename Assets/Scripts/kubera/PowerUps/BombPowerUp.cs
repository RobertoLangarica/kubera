﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class BombPowerUp : PowerupBase 
{
	public AnimatedSprite Animation;
	public GameObject powerUpBlock;
	public Transform powerUpButton;

	protected CellsManager cellsManager;
	protected InputBombAndDestroy bombInput;
	protected WordManager wordManager;

	protected GameObject bombGO;

	protected List<AnimatedSprite> freeAnimation = new List<AnimatedSprite>();
	protected List<AnimatedSprite> occupiedAnimation = new List<AnimatedSprite>();

	void Start () 
	{
		cellsManager = FindObjectOfType<CellsManager>();
		bombInput = FindObjectOfType<InputBombAndDestroy>();
		wordManager = FindObjectOfType<WordManager>();

		for(int i=0; i<8; i++)
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

	public override void activate()
	{
		if (bombGO != null) 
		{
			DestroyImmediate (bombGO);
		}
		bombGO = Instantiate (powerUpBlock,powerUpButton.position,Quaternion.identity) as GameObject;
		bombGO.name = "BombPowerUp";
		bombGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);

		bombInput.enabled = true;
		bombInput.setCurrentSelected(bombGO);
		bombInput.OnDrop += powerUpPositioned;
	}

	public void powerUpPositioned()
	{
		Cell cellSelected = cellsManager.getCellUnderPoint(bombGO.transform.position);

		if(cellSelected != null)
		{
			if(cellSelected.contentType == Piece.EType.PIECE && cellSelected.occupied)
			{
				StartCoroutine (startAnim (cellSelected));

				DestroyImmediate(bombGO);
				bombInput.OnDrop -= powerUpPositioned;
				bombInput.enabled = false;
				OnComplete ();
			}
			else 
			{
				powerUPCanceled();
			}
		}
		else
		{
			powerUPCanceled();
		}
	}

	public void powerUPCanceled()
	{
		bombGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("BombPowerUP_Move");
		bombGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("BombPowerUP_Scale").OnComplete (() => {

			DestroyImmediate (bombGO);
		});

		bombInput.OnDrop -= powerUpPositioned;
		bombInput.enabled = false;

		OnCancel();
	}

	IEnumerator startAnim(Cell cellSelected)
	{
		Cell[] selection =  cellsManager.getCellNeighborsOfSameColor(cellSelected);
		List<Cell> selectionList = new List<Cell>();
		for(int i=0; i<selection.Length; i++)
		{
			selectionList.Add (selection [i]);
		}

		while (selectionList.Count >0)
		{
			int random = Random.Range (0, selectionList.Count);
			yield return new WaitForSeconds (0.1f);

			Square square = selectionList [random].content.GetComponent<Square> ();

			Letter letter = wordManager.getGridLetterFromPool (WordManager.EPoolType.NORMAL);
			letter.enabled = false;
			StartCoroutine (startAnimation (square, selectionList [random], letter, 0.1f));
			selectionList.Remove (selectionList [random]);

		}

	}

	IEnumerator startAnimation(Square square,Cell cellParent,Letter letter,float delay)
	{
		AnimatedSprite animSprite = getFreeAnimation ();
		letter.enabled = false;

		Vector3 cellPosition =  cellParent.transform.position + (new Vector3 (cellParent.GetComponent<SpriteRenderer> ().bounds.extents.x,
			-cellParent .GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

		animSprite.gameObject.transform.position = cellPosition;
		animSprite.gameObject.SetActive(true);

		yield return new WaitForSeconds (delay);

		animSprite.enabled = true;
		animSprite.autoUpdate = true;
		yield return new WaitUntil (()=> animSprite.sequences[0].currentFrame >= 6);


		//yield return new WaitUntil (()=> animSprite.sequences[0].currentFrame >= 11);

		animSprite.enabled = false;
		animSprite.autoUpdate = false;
		animSprite.sequences [0].currentFrame = 0;
		releaseAnimation (animSprite);
		yield return new WaitForSeconds (0.1f);
		square.OnCellFlipped += callbackOnFliped;
		square.doFlip (cellParent, letter, delay);
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
		cellsManager.occupyAndConfigureCell(cell,letter.gameObject,Piece.EType.LETTER,Piece.EColor.NONE,true);
	}
}