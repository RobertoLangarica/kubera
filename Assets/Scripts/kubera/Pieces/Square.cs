using UnityEngine;
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
	void Start () 
	{
		flipAnimation.enabled = false;
		flash.enabled = false;
		flash.spriteRenderer = spriteRenderer;
	}

	public void doDoubleFlash()
	{
		flash.enabled = true;


		flash.startFlash();
		flash.onFinish += secondFlash;
	}


	public void secondFlash()
	{
		flash.startFlash();
		flash.onFinish += secondFlash;
	}

	private void onDoubleFlashCompleted()
	{
		//Callback	
	}

	public void doFlip(Cell cellParent, Letter letterAfterFlip, float delay )
	{
		letterAfterFlip.gameObject.SetActive(false);
		this.cellParent = cellParent;
		letter = letterAfterFlip;

		Invoke("startFlipAnimation");
	}

	IEnumerator startFlipAnimation(float delayTime)
	{
		yield return new WaitForSeconds (delayTime);

		flipAnimation.enabled = true;
		flipAnimation.autoUpdate = true;

		yield return new WaitUntil (()=>flipAnimation.sequences[0].currentFrame >= 14);
		cellParent.content.GetComponent<SpriteRenderer> ().color = Color.white;

		yield return new WaitUntil (()=>flipAnimation.sequences[0].currentFrame >= 23);

		Vector3 cellPosition =  cellParent .transform.position + (new Vector3 (cellParent.GetComponent<SpriteRenderer> ().bounds.extents.x,
			-cellParent .GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

		letter.gameObject.transform.position = cellPosition;
		letter.gameObject.SetActive(true);

		yield return new WaitUntil (()=> flipAnimation.sequences[0].currentFrame >= 26);

		flipAnimation.enabled = false;
		flipAnimation.autoUpdate = false;

		OnCellFlipped (this, cellParent,letter);
	}
}
