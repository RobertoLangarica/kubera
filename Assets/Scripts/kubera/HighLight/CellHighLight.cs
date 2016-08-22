using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellHighLight : HighLight 
{
	public const string PIECE_LAYER 		= "Piece";
	public const string OVER_PIECE_LAYER 	= "ShadowPiece";

	public Sprite square;
	public Sprite roundedSquare;

	protected List<HighLightManager.EHighLightType> squareCases;

	protected override void updateColor ()
	{
		isScaled = true;

		base.updateColor ();

		SpriteRenderer tempSpt = GetComponent<SpriteRenderer> ();

		if(squareCases == null)
		{
			populateList ();
		}


		for (int i = 0; i < squareCases.Count; i++) 
		{
			if (suscribedTypes [suscribedTypes.Count - 1] == squareCases [i]) 
			{
				tempSpt.sprite = square;
				tempSpt.sortingLayerName = PIECE_LAYER;
				tempSpt.sortingOrder = -10;
				transform.localScale = new Vector3 (1,1,1);
				setScale ();
				return;
			}
		}
			
		tempSpt.sprite = roundedSquare;
		tempSpt.sortingLayerName = OVER_PIECE_LAYER;
		tempSpt.sortingOrder = -10;
		transform.localScale = new Vector3 (1,1,1);
		setScale ();
	}

	protected void populateList()
	{
		squareCases = new List<HighLightManager.EHighLightType> ();

		squareCases.Add (HighLightManager.EHighLightType.EMPTY_CELLS);
		squareCases.Add (HighLightManager.EHighLightType.SQUARE_POWERUP);
	}

}
