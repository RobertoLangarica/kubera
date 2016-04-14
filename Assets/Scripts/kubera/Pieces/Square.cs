using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour 
{

	public FlashColor flash;
	public AnimatedSprite flipAnimation;
	public SpriteRenderer spriteRenderer;

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

	public doFlip(Cell cellParent, Letter letterAfterFlip, float delay )
	{
		letterAfterFlip.gameObject.SetActive(false);


		Invoke("flipAnimation")
	}

	IEnumerator flipAnimation()
	{
		yield return new WaitForSeconds (delayTime);

		flipAnimation.enabled = true;
		flipAnimation.autoUpdate = true;

		yield return new WaitUntil (()=>animSprite.sequences[0].currentFrame >= 14);
		cell.content.GetComponent<SpriteRenderer> ().color = Color.white;

		yield return new WaitUntil (()=>animSprite.sequences[0].currentFrame >= 23);

		Vector3 cellPosition =  cell .transform.position + (new Vector3 (cell.GetComponent<SpriteRenderer> ().bounds.extents.x,
			-cell .GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

		letter.gameObject.transform.position = cellPosition;
		letter.gameObject.SetActive(true);

		yield return new WaitUntil (()=> animSprite.sequences[0].currentFrame >= 26);

		animSprite.enabled = false;
		animSprite.autoUpdate = false;

		OnCellFlipped (this,cell,letter);
	}
}
