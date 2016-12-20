using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class DestroyPowerUp : PowerupBase 
{
	public CellsManager cellsManager;
	public InputBombAndDestroy bombInput;
	public WordManager wordManager;
	public GameManager gameManager;
	public BombAnimation destroyAnim;

	protected bool canUse;
	protected GameObject destroyGO;

	protected Cell highLightCell;

	void Start () 
	{
		destroyAnim.OnCellFlipped += gameManager.OnCellFlipped;
		destroyAnim.OnAllAnimationsCompleted += gameManager.updatePiecesLightAndUpdateLetterState;
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
		this.canUse = canUse;

		destroyGO.GetComponentInChildren<SpriteRenderer> ().sortingLayerName = "Selected";
		updateDragableObjectImage (destroyGO);

		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.DESTROY_POWERUP);

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Play("ray");
		}
	}

	public void powerUpPositioned()
	{
		Cell cellSelected = cellsManager.getCellUnderPoint(destroyGO.transform.position);

		if(AudioManager.GetInstance())
		{
			AudioManager.GetInstance().Stop("ray");
		}

		if(cellSelected != null)
		{
			if(cellSelected.contentType == Piece.EType.PIECE && cellSelected.occupied)
			{
				if(canUse)
				{
					StartCoroutine (destroyAnim.destroyColorSearchAnimation(cellSelected));
					DestroyImmediate(destroyGO);
					bombInput.OnDrop -= powerUpPositioned;
					bombInput.enabled = false;

					HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.DESTROY_POWERUP);

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

		OnCancel();
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

		OnCompletedNoGems ();
	}
}
