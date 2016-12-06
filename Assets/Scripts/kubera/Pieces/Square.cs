﻿using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour 
{
	public delegate void DOnFlipComplete(Square square, Cell cell,Letter letter);
	public DOnFlipComplete OnCellFlipped;

	public FlashColor flash;
	public AnimatedSprite flipAnimation;
	public SpriteRenderer spriteRenderer;

	protected Cell cellParent;
	protected Letter letter;

	//TODO: hardcoding
	public bool oneSquare;

	void Start () 
	{
		flipAnimation.enabled = false;
		flash.enabled = false;
		flash.spriteRenderer = spriteRenderer;
	}

	private void onDoubleFlashCompleted()
	{
		//flash.enabled = false;
	}

	public void doFlip(Cell cellParent, Letter letterAfterFlip, float delay )
	{
		letterAfterFlip.gameObject.SetActive(false);
		this.cellParent = cellParent;
		letter = letterAfterFlip;

		if (flipAnimation.spriteRenderer == null) 
		{
			return;
		}

		if (cellParent.sprite_renderer.sortingLayerName == "Modal") 
		{
			flipAnimation.spriteRenderer.sortingLayerName = "WebView";
		}
		else 
		{
			flipAnimation.spriteRenderer.sortingLayerName = "Flipping";
		}

		StartCoroutine (startFlipAnimation (delay*0.01f));
	}

	IEnumerator startFlipAnimation(float delayTime)
	{
		yield return new WaitForSeconds (delayTime);

		flipAnimation.enabled = true;
		flipAnimation.autoUpdate = true;

		//TODO: HardCodding pedirle a lilo que sean del mismo tamaño
		yield return new WaitUntil (()=>flipAnimation.sequences[0].currentFrame >= 1);
		if(!oneSquare)
		{
			this.transform.localScale = new Vector3 (2.2f, 2.2f, 2.2f);
		}
		else
		{
			this.transform.localScale = new Vector3 (0.58f, 0.58f, 0.58f);
		}
		yield return new WaitUntil (()=>flipAnimation.sequences[0].currentFrame >= 14);
		cellParent.content.GetComponent<SpriteRenderer> ().color = Color.white;

		yield return new WaitUntil (()=>flipAnimation.sequences[0].currentFrame >= 23);

		Vector3 cellPosition =  cellParent .transform.position + (new Vector3 (cellParent.GetComponent<SpriteRenderer> ().bounds.extents.x,
			-cellParent .GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

		letter.gameObject.transform.position = cellPosition;
		letter.gameObject.SetActive(true);

		OnCellFlipped (this, cellParent,letter);
		yield return new WaitUntil (()=> flipAnimation.sequences[0].currentFrame >= 26);

		flipAnimation.enabled = false;
		flipAnimation.autoUpdate = false;


	}
}
