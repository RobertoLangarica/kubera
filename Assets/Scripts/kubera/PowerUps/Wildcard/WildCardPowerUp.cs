using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WildCardPowerUp : PowerupBase
{
	public string powerUpScore = "x3";
	public RectTransform wordsContainer;

	public KeyBoardManager keyBoard;

	protected CellsManager cellsManager;
	protected InputWildcardPowerUp powerUpInput;
	protected GameManager gameManager;
	protected WordManager wordManager;
	public AnimatedSprite animatedSprite;

	protected bool canUse;
	protected bool canActivate;
	protected GameObject powerUpGO;
	void Start()
	{
		wordManager = FindObjectOfType<WordManager> ();
		gameManager = FindObjectOfType<GameManager> ();

		powerUpInput = FindObjectOfType<InputWildcardPowerUp> ();
		powerUpInput.OnPowerupCanceled += cancel;
		powerUpInput.OnPowerupCompleted += completePowerUp;
		powerUpInput.OnPowerupCompletedNoGems += completePowerUpNoGems;

		powerUpInput.enabled = false;
		gameManager = FindObjectOfType<GameManager> ();
	}

	public override void activate (bool canUse)
	{
		powerUpInput.enabled = true;

		powerUpInput.createBlock (powerUpBlock,powerUpButton.position,canUse);

		updateDragableObjectImage (powerUpInput.getCurrentSelected());

		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.WILDCARD_POWERUP);
	}

	protected void completePowerUp()
	{
		powerUpInput.enabled = false;
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WILDCARD_POWERUP);
		
		gameManager.updatePiecesLightAndUpdateLetterState ();
		OnComplete ();
	}

	protected void completePowerUpNoGems()
	{
		powerUpInput.enabled = false;
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WILDCARD_POWERUP);
		OnCompletedNoGems ();
	}

	public override void cancel()
	{
		powerUpInput.enabled = false;
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.WILDCARD_POWERUP);
		OnCancel ();
	}
}
