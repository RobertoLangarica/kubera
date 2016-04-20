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

		flash.startFlash(spriteRenderer,0.1f);
		flash.onFinish += secondFlash;
	}


	public void secondFlash()
	{
		flash.onFinish -= secondFlash;
		flash.startFlash(spriteRenderer,0.3f);
		flash.onFinish += onDoubleFlashCompleted;
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

		StartCoroutine (startFlipAnimation (delay*0.2f));
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
