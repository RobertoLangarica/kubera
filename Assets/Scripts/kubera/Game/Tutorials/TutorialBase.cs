using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class TutorialBase : MonoBehaviour 
{
	public enum ENextPhaseEvent
	{
		CREATE_WORD,
		SUBMIT_WORD,
		DELETE_WORD,
		CREATE_A_LINE,
		WORD_SPECIFIC_LETTER_TAPPED,
		GRID_SPECIFIC_LETTER_TAPPED,
		KEYBOARD_SPECIFIC_LETER_SELECTED,
		PIECE_ROTATED,
		TAP,
		BOMB_USED,
		BLOCK_USED,
		ROTATE_USED,
		DESTROY_USED,
		WILDCARD_USED
	}

	[HideInInspector]public bool allowGridTap;
	[HideInInspector]public bool allowWordTap;
	[HideInInspector]public bool allowLetterDrag;
	[HideInInspector]public bool allowErraseWord;
	[HideInInspector]public bool allowDragPieces;
	[HideInInspector]public bool allowPowerUps;

	//PowerUps
	[HideInInspector]public bool freeBombs;
	[HideInInspector]public bool freeBlocks;
	[HideInInspector]public bool freeRotates;
	[HideInInspector]public bool freeDestroy;
	[HideInInspector]public bool freeWildCard;

	public string levelName;

	public List<Text> instructions;

	public List<GameObject> phasesPanels;

	public List<GameObject> handPositions;

	public GoalPopUp goalPopUp;
	public AnimatedSprite tutorialHand;
	public GameObject handObject;

	[HideInInspector]public int phase;
	[HideInInspector]public string phaseObj;
	[HideInInspector]public ENextPhaseEvent phaseEvent;

	public delegate void DAnimationNotification ();

	protected bool alphaLowered;
	protected DAnimationNotification OnMovementComplete;
	protected WordManager wordManager;
	protected GameManager gameManager;
	protected CellsManager cellManager;

	protected virtual void Start()
	{
		gameManager = FindObjectOfType<GameManager> ();
		wordManager = FindObjectOfType<WordManager> ();
		cellManager = FindObjectOfType<CellsManager> ();

		scaleHand ();
	}

	void Update()
	{
		if(Input.touchCount > 0 || Input.GetMouseButton(0))
		{
			if (!alphaLowered) 
			{
				Debug.Log ("1");
				changeAlpha (0.3f, 0.3f);
			}
		}
		else if(alphaLowered)
		{
			Debug.Log ("2");
			changeAlpha (1,0.5f);
			alphaLowered = false;
		}
		/*if (Input.GetKeyDown (KeyCode.Q)) 
		{
			playPressAnimation ();
		}
		if (Input.GetKeyDown (KeyCode.W)) 
		{
			playReleaseAnimation ();
		}
		if (Input.GetKeyDown (KeyCode.E)) 
		{
			playTapAnimation ();
		}*/
	}

	protected void scaleHand()
	{
		float nScale = 0;

		nScale = (Screen.height * 0.01f) * 0.3f;
		nScale = ((nScale * 100) / (tutorialHand.gameObject.GetComponent<SpriteRenderer> ().bounds.size.y)) * 0.01f;

		tutorialHand.transform.localScale = new Vector3 (nScale,nScale,nScale);
	}

	protected void hideObject()
	{
		handObject.SetActive (false);
	}

	protected void hideHand()
	{
		tutorialHand.gameObject.SetActive (false);

		if (handObject != null) 
		{
			handObject.SetActive (false);
		}
	}

	protected void showObjectAtHand(Vector3 objOffset)
	{
		handObject.SetActive (true);
		handObject.transform.position = tutorialHand.transform.position + objOffset;
	}

	protected void showHandAt(Vector3 pos,Vector3 objOffset,bool showObjt = true)
	{
		tutorialHand.transform.position = pos;
		tutorialHand.gameObject.SetActive (true);

		if (handObject != null && showObjt) 
		{
			showObjectAtHand (objOffset);
		}
	}

	protected void moveHandFromGameObjects(GameObject goFrom,GameObject goTo,Vector3 objOffset,float time = 1)
	{
		tutorialHand.transform.position = goFrom.transform.position;
		
		tutorialHand.transform.DOMove (goTo.transform.position, time).OnComplete(()=>{if(OnMovementComplete!=null){OnMovementComplete();}}).SetId("handMovement");

		if (handObject != null) 
		{
			handObject.transform.DOMove (goTo.transform.position + objOffset, time).SetId ("objectMovement");
		}
	}

	protected void finishMovements()
	{
		DOTween.Kill ("handMovement");
		DOTween.Kill ("objectMovement");
	}

	protected void changeAlpha(float nHandValue,float nObjtValue)
	{
		alphaLowered = true;

		tutorialHand.GetComponent<SpriteRenderer> ().color = changeColorAlpha(tutorialHand.GetComponent<SpriteRenderer> ().color, nHandValue);

		if (handObject != null) 
		{
			if (handObject.GetComponent<Image> () != null) 
			{
				handObject.GetComponent<Image> ().color = changeColorAlpha (handObject.GetComponent<Image> ().color, nObjtValue);
			} 
			else if (handObject.GetComponent<SpriteRenderer> () != null) 
			{
				handObject.GetComponent<SpriteRenderer> ().color = changeColorAlpha (handObject.GetComponent<SpriteRenderer> ().color, nObjtValue);
			} 
			else 
			{
				SpriteRenderer[] temp = handObject.GetComponentsInChildren<SpriteRenderer> ();

				for (int i = 0; i < temp.Length; i++) 
				{
					temp [i].color = changeColorAlpha (temp [i].color, nObjtValue);
				}
			}
		}
	}

	private Color changeColorAlpha(Color col,float val)
	{
		Color temp = col;

		temp.a = val;

		return temp;
	}

	protected IEnumerator playPressAndContinueWithMethod(string methodName,float time)
	{
		yield return new WaitForSeconds(0.7f);
		playPressAnimation ();
		Invoke (methodName, time);
	}

	protected void playPressAnimation()
	{
		tutorialHand.autoUpdate = false;
		tutorialHand.sequences [0].currentFrame = 1;
		tutorialHand.sequences [0].updateImage();
	}

	protected void playReleaseAnimation()
	{
		tutorialHand.autoUpdate = false;
		tutorialHand.sequences [0].currentFrame = 0;
		tutorialHand.sequences [0].updateImage();
	}

	protected void playTapAnimation()
	{
		tutorialHand.sequences [0].currentFrame = 0;
		tutorialHand.sequences [0].updateImage();
		tutorialHand.autoUpdate = true;
	}

	public virtual bool canMoveToNextPhase()
	{
		return false;
	}

	public virtual bool phaseObjectiveAchived()
	{
		return false;
	}
}