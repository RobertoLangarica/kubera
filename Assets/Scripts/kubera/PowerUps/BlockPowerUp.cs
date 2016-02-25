using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BlockPowerUp : PowerupBase
{
	protected InputBlockPowerUp inputBlockPowerUp;

	public GameObject powerUpBlock;
	public Transform buttonPowerUpBlock;

	void Start()
	{
		inputBlockPowerUp = FindObjectOfType<InputBlockPowerUp> ();
		inputBlockPowerUp.OnPowerupCanceled += cancelPowerUp;
		inputBlockPowerUp.OnPowerupCompleted += completePowerUp;

		this.gameObject.SetActive( false);
	}

	public override void activate ()
	{
		this.gameObject.SetActive( true);

		inputBlockPowerUp.createBlock (powerUpBlock,buttonPowerUpBlock.position);
	}

	protected void completePowerUp()
	{
		OnComplete ();
	}

	protected void cancelPowerUp()
	{
		OnCancel ();
	}
}
