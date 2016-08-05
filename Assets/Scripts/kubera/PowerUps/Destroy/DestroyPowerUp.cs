using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class DestroyPowerUp : PowerupBase 
{
	public AnimatedSprite Animation;

	protected CellsManager cellsManager;
	protected InputBombAndDestroy bombInput;
	protected WordManager wordManager;
	protected GameManager gameManager;

	protected GameObject destroyGO;

	protected List<AnimatedSprite> freeAnimation = new List<AnimatedSprite>();
	protected List<AnimatedSprite> occupiedAnimation = new List<AnimatedSprite>();
	protected bool canUse;

	protected Cell highLightCell;

	void Start () 
	{
		cellsManager = FindObjectOfType<CellsManager>();
		bombInput = FindObjectOfType<InputBombAndDestroy>();
		wordManager = FindObjectOfType<WordManager> ();
		gameManager = FindObjectOfType<GameManager> ();

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

	public override void activate(bool canUse)
	{
		if (destroyGO != null) 
		{
			DestroyImmediate (destroyGO);
		}
		destroyGO = Instantiate (powerUpBlock,powerUpButton.position,Quaternion.identity) as GameObject;
		destroyGO.name = "DestroyPowerUp";
		destroyGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);
		destroyGO.transform.localScale = new Vector3 (4, 4, 4);

		bombInput.enabled = true;
		bombInput.setCurrentSelected(destroyGO);
		bombInput.OnDrop += powerUpPositioned;
		bombInput.OnCellSelected += onOverCellChanged;
		this.canUse = canUse;

		destroyGO.GetComponentInChildren<SpriteRenderer> ().sortingLayerName = "Selected";
		updateDragableObjectImage (destroyGO);

		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.DESTROY_POWERUP);
	}

	public void powerUpPositioned()
	{
		bombInput.OnCellSelected -= onOverCellChanged;
		Cell cellSelected = cellsManager.getCellUnderPoint(destroyGO.transform.position);

		if(cellSelected != null)
		{
			if(cellSelected.contentType == Piece.EType.PIECE && cellSelected.occupied)
			{
				if(canUse)
				{
					StartCoroutine (startAnim (cellSelected));
					DestroyImmediate(destroyGO);
					bombInput.OnDrop -= powerUpPositioned;
					bombInput.enabled = false;

					HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_POWERUP);
					HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_SPECIFIC_COLOR);

					OnComplete ();
				}
				else
				{
					onCompletedNoGems ();
				}

			}
			else 
			{
				cancel();
			}
		}
		else
		{
			cancel();
		}
	}

	public override void cancel()
	{
		destroyGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("DestroyPowerUP_Move");
		destroyGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId("DestroyPowerUP_Scale").OnComplete (() => {

			DestroyImmediate(destroyGO);
		});

		bombInput.OnDrop -= powerUpPositioned;
		bombInput.enabled = false;

		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_POWERUP);
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_SPECIFIC_COLOR);

		OnCancel();
	}

	IEnumerator startAnim(Cell cellSelected)
	{
		Cell[] selection =  cellsManager.getCellsOfSameColor(cellSelected);
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
			letter.gameObject.SetActive(false);

			StartCoroutine (startAnimation (square, selectionList [random], letter, 0.1f));
			selectionList.Remove (selectionList [random]);

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
		gameManager.OnCellFlipped (cell, letter);
	}

	public void onCompletedNoGems()
	{
		destroyGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId("DestroyPowerUP_Move");
		destroyGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId("DestroyPowerUP_Scale").OnComplete (() => {

			DestroyImmediate(destroyGO);
		});

		bombInput.OnDrop -= powerUpPositioned;
		bombInput.enabled = false;

		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_POWERUP);
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_SPECIFIC_COLOR);

		OnCompletedNoGems ();
	}

	public void onOverCellChanged(Cell cellSelected)
	{
		if (cellSelected != null) 
		{
			if (highLightCell == null) 
			{
				highLightCell = cellSelected;
			}

			if (cellSelected.contentColor != highLightCell.contentColor) 
			{
				HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_SPECIFIC_COLOR);
			}


			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.DESTROY_SPECIFIC_COLOR,cellSelected);
			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_POWERUP);

			highLightCell = cellSelected;
		} 
		else 
		{
			HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.DESTROY_POWERUP);
			HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_SPECIFIC_COLOR);
		}
	}
}
