using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LinesCreatedAnimation : MonoBehaviour 
{
	public delegate void DAnimationFinished(Cell cell,Letter letter, Piece.EType type, Piece.EColor color);

	public DAnimationFinished OnAnimationFinish;

	public float betweenFlashTime = 0.1f;
	public float cellsDelayTime = 0.5f;

	protected List<Cell> cellsToAnimate;
	protected CellsManager cellManager;
	protected WordManager wordManager;

	public void configurateAnimation(List<Cell> cells,CellsManager cellM,WordManager wordM)
	{
		cellsToAnimate = new List<Cell> (cells);
		cellManager = cellM;
		wordManager = wordM;

		for (int i = 0; i < cellsToAnimate.Count; i++) 
		{
			StartCoroutine (startFlashPiece(cellsToAnimate[i]));
		}
	}

	IEnumerator startFlashPiece(Cell cell)
	{
		FlashColor flashColor = cell.content.GetComponent<FlashColor> ();
		SpriteRenderer spriteRenderer = cell.content.GetComponent<SpriteRenderer> ();

		yield return new WaitForSeconds (betweenFlashTime);
		flashColor.startFlash (spriteRenderer,0.1f);
		yield return new WaitForSeconds (betweenFlashTime * 2);
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

			animSprite.enabled = false;
			animSprite.autoUpdate = false;

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