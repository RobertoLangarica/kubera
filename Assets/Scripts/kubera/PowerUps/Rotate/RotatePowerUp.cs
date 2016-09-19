using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RotatePowerUp : PowerupBase
{
	public RectTransform piecesPanel;

	protected InputPowerUpRotate inputPowerUpRotate;
	protected InputBombAndDestroy inputPowerUp;
	protected InputWords inputWords;
	protected GameManager gameManager;

	protected GameObject powerUpGO;
	protected bool canUse;
	protected Transform[] imageToRotate;
	protected Vector3 initiaImageToRotateScale;

	void Start()
	{
		inputWords = FindObjectOfType<InputWords> ();
		inputPowerUpRotate = FindObjectOfType<InputPowerUpRotate> ();
		inputPowerUp = FindObjectOfType<InputBombAndDestroy> ();
		gameManager = FindObjectOfType<GameManager> ();

		this.gameObject.SetActive( false);
		inputPowerUpRotate.enabled = false;
		inputPowerUpRotate.gameObject.SetActive (false);

		imageToRotate = FindObjectOfType<HUDManager>().rotationImagePositions;
		initiaImageToRotateScale = imageToRotate [0].transform.localScale;
	}

	public override void activate (bool canUse)
	{
		this.gameObject.SetActive( true);
		if (powerUpGO != null) 
		{
			DestroyImmediate (powerUpGO);
		}
		powerUpGO = Instantiate (powerUpBlock,powerUpButton.position,Quaternion.identity) as GameObject;
		powerUpGO.name = "RotatePowerUp";
		powerUpGO.transform.position = new Vector3(powerUpButton.position.x,powerUpButton.position.y,0);
		powerUpGO.transform.localScale = new Vector3 (4, 4, 4);
		powerUpGO.GetComponentInChildren<SpriteRenderer> ().sortingLayerName = "Selected";
		inputPowerUp.enabled = true;
		inputPowerUp.setCurrentSelected(powerUpGO);
		inputPowerUp.OnDrop += powerUpPositioned;

		inputWords.allowInput = true;
		this.canUse = canUse;

		updateDragableObjectImage (powerUpGO);

		HighLightManager.GetInstance ().setHighLightOfType (HighLightManager.EHighLightType.ROTATE_POWERUP);
	}

	public void powerUpPositioned()
	{
		bool activated = false;
		Vector3 v3 = new Vector3();
		v3 = powerUpGO.transform.position;
		v3 = Camera.main.WorldToScreenPoint (v3);

		v3.x = v3.x/Screen.width;

		if (v3.x > piecesPanel.anchorMin.x && v3.x < piecesPanel.anchorMax.x) 
		{
			v3.y = v3.y/Screen.height;
			if (v3.y > piecesPanel.anchorMin.y && v3.y < piecesPanel.anchorMax.y) 
			{
				activated = true;
			}
		}
		//print ("powerUpPositioned "+ activated);
		if (!activated) 
		{
			powerUpGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId ("RotatePowerUP_Move");
			powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("RotatePowerUP_Scale").OnComplete (() => {

				DestroyImmediate (powerUpGO);
				cancel ();
			});
		}
		else 
		{
			if(!canUse)
			{
				powerUpGO.transform.DOMove (new Vector3 (powerUpButton.position.x, powerUpButton.position.y, 1), .2f).SetId ("RotatePowerUP_Move");
			}

			powerUpGO.transform.DOScale (new Vector3 (0, 0, 0), .2f).SetId ("RotatePowerUP_Scale").OnComplete (() => {

				DestroyImmediate (powerUpGO);
				powerUpActivateRotate (canUse);
			});

		}
		inputPowerUp.OnDrop -= powerUpPositioned;
		HighLightManager.GetInstance ().turnOffHighLights (HighLightManager.EHighLightType.ROTATE_POWERUP);
	}

	public void powerUpActivateRotate(bool canUse)
	{
		inputPowerUp.OnDrop -= powerUpPositioned;
		if(canUse)
		{
			if(inputPowerUpRotate.gameObject.activeSelf)
			{
				cancel ();
				return;
			}
			inputPowerUpRotate.gameObject.SetActive (true);
			inputPowerUpRotate.enabled = true;
			inputPowerUpRotate.startRotate ();
			inputPowerUpRotate.OnPowerupRotateCompleted += completePowerUp;
			print ("activate");
			gameManager.updatePiecesLightAndUpdateLetterState ();
			Invoke ("rotateImage", 2);

			if(AudioManager.GetInstance())
			{
				AudioManager.GetInstance().Stop("startRotate");
				AudioManager.GetInstance().Play("startRotate");
			}
		}
		else
		{
			OnCompletedNoGems ();
		}
	}

	protected void completePowerUp()
	{
		inputPowerUpRotate.OnPowerupRotateCompleted -= completePowerUp;
		this.gameObject.SetActive( false);
		inputPowerUpRotate.enabled = false;
		inputPowerUpRotate.gameObject.SetActive (false);
		cancelInvoke ();
		OnComplete ();
	}

	public override void cancel()
	{
		//print ("cancelPowerUp");
		OnCancel ();
		inputPowerUp.enabled = false;


		if(inputPowerUpRotate.gameObject.activeSelf)
		{
			return;
		}
		inputPowerUpRotate.enabled = false;
		this.gameObject.SetActive( false);
	}

	protected void rotateImage()
	{
		for(int i=0; i<imageToRotate.Length; i++)
		{
			imageToRotate [i].DOScale (new Vector2 (imageToRotate [i].localScale.x + 0.1f, imageToRotate [i].localScale.y + 0.1f), 0.5f).OnComplete(()=>
				{
					imageToRotate [i].DOScale (new Vector2 (imageToRotate [i].localScale.x - 0.1f, imageToRotate [i].localScale.y - 0.1f), 0.5f).OnComplete(()=>
						{
						}).SetId("imageToRotate");
				}).SetId("imageToRotate");
			
			imageToRotate [i].DOLocalRotate (new Vector3 (0, 0, imageToRotate [i].localRotation.z - 90), 1.0f,RotateMode.LocalAxisAdd).SetId ("imageToRotate");
		}
		Invoke ("scaleRotateImage", 0.5f);
	}

	protected void scaleRotateImage()
	{
		for(int i=0; i<imageToRotate.Length; i++)
		{
			imageToRotate [i].DOScale (new Vector2 (imageToRotate [i].localScale.x - 0.1f, imageToRotate [i].localScale.y - 0.1f), 0.5f).SetId("imageToRotate");;
		}
		Invoke ("rotateImage", Random.Range(2,5));
	}

	protected void cancelInvoke()
	{
		CancelInvoke ("rotateImage");
		CancelInvoke ("scaleRotateImage");
		DOTween.Kill ("imageToRotate");
		for(int i=0; i<imageToRotate.Length; i++)
		{
			imageToRotate [i].transform.localScale = initiaImageToRotateScale;
		}
	}
}

