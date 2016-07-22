using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LinesCreatedAnimation : MonoBehaviour 
{
	public delegate void DOnAnimationFinish(Cell cell,Letter letter);

	public DOnAnimationFinish OnCellFlipped;
	public bool isOnAnimation;

	public float betweenFlashTime = 0.05f;
	public float cellsDelayTime = 0.5f;

	protected List<Cell> cellsToAnimate;

	public void configurateAnimation(List<Cell> cells,List<Letter> letters)
	{
		cellsToAnimate = new List<Cell> (cells);

		for (int i = 0; i < cellsToAnimate.Count; i++) 
		{
			Square square = cellsToAnimate [i].content.GetComponent<Square> ();
			letters [i].gameObject.SetActive (false);
			isOnAnimation = true;
			StartCoroutine (startFlashPiece(square,cellsToAnimate[i],letters[i]));
		}
	}

	IEnumerator startFlashPiece(Square square, Cell cellParent, Letter letterContent)
	{
		square.flash.enabled = true;

		yield return new WaitForSeconds (betweenFlashTime);
		square.flash.startFlash (square.spriteRenderer,1f);
		yield return new WaitForSeconds (betweenFlashTime * 2);
		square.flash.startFlash (square.spriteRenderer,0.01f);
		yield return new WaitForSeconds (betweenFlashTime);


		//TODO:Iniciar flip en Square
		//TODO: escuchar callback onFlipped de square
		//TODO: Mandar callback hacia afuer
		square.OnCellFlipped += flipCallBack;
		square.doFlip(cellParent,letterContent,cellsDelayTime * (cellsToAnimate.IndexOf(cellParent)+1));
		//StartCoroutine( startAnimationFlipPiece (cell.content,cell,cellsDelayTime * (cellsToAnimate.IndexOf(cell)+1)));
	}

	public void flipCallBack(Square square, Cell cellParent, Letter letterContent)
	{
		square.OnCellFlipped -= flipCallBack;

		if (cellsToAnimate.IndexOf (cellParent) == (cellsToAnimate.Count - 1)) 
		{
			isOnAnimation = false;
		}
		OnCellFlipped (cellParent,letterContent);
	}

	/*IEnumerator startAnimationFlipPiece(GameObject obj, Cell cell,float delayTime)

		flashColor.startFlash (spriteRenderer,0.3f);

		yield return new WaitForSeconds (betweenFlashTime);
		StartCoroutine( startAnimationFlipPiece (cell.content,cell,cellsDelayTime * (cellsToAnimate.IndexOf(cell)+1)));
	}

	IEnumerator startAnimationFlipPiece(GameObject obj, Cell cell,float delayTime)
	{
		AnimatedSprite animSprite = obj.GetComponent < AnimatedSprite> ();
		yield return new WaitForSeconds (delayTime);

		if(animSprite)
		{
			Letter letter = wordManager.getGridLetterFromPool(WordManager.EPoolType.NORMAL);
			letter.gameObject.SetActive(false);
			animSprite.enabled = true;
			animSprite.autoUpdate = true;

			yield return new WaitUntil (()=>animSprite.sequences[0].currentFrame >= 14);
			cell.content.GetComponent<SpriteRenderer> ().color = Color.white;

			yield return new WaitUntil (()=>animSprite.sequences[0].currentFrame >= 23);

			Vector3 cellPosition =  cell .transform.position + (new Vector3 (cell.GetComponent<SpriteRenderer> ().bounds.extents.x,
				-cell .GetComponent<SpriteRenderer> ().bounds.extents.x, 0));

			letter.gameObject.transform.position = cellPosition;
			letter.gameObject.SetActive(true);

			yield return new WaitUntil (()=> animSprite.sequences[0].currentFrame >= 26);

			Debug.Log (isOnAnimation);


			OnAnimationFinish (cell,letter,Piece.EType.LETTER,Piece.EColor.NONE);
		}
	}

	/*public List<Cell> orderCellsLikeAWave(List<Cell> linesCells,Cell originCell)
	{
		List<Cell> orderedCells = new List<Cell>();
		List<Vector2> linesCellsXY = new List<Vector2>();
		Vector2 originXY = cellManager.getCellXYPosition (originCell);
		Vector2 tempXY = Vector2.zero;
		int count = 0;
		int counter = 1;

		orderedCells.Add (originCell);

		for (int i = 0; i < linesCells.Count; i++) 
		{
			linesCellsXY.Add (cellManager.getCellXYPosition(linesCells[i]));
		}

		linesCellsXY.Remove (originXY);

		while (count < cellManager.columns) 
		{
			for (int i = 0; i < counter; i++) 
			{
			}
		}

		return orderedCells;
	}*/
}