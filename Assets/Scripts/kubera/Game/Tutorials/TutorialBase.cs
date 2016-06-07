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

	public AnimatedSprite tutorialHand;

	[HideInInspector]public int phase;
	[HideInInspector]public string phaseObj;
	[HideInInspector]public ENextPhaseEvent phaseEvent;

	public delegate void DAnimationNotification ();

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

	/*void Update()
	{
		if (Input.GetKeyDown (KeyCode.Q)) 
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
		}
	}*/

	protected void scaleHand()
	{
		float nScale = 0;

		nScale = (Screen.height * 0.01f) * 0.2f;
		nScale = ((nScale * 100) / (tutorialHand.gameObject.GetComponent<SpriteRenderer> ().bounds.size.y)) * 0.01f;

		tutorialHand.transform.localScale = new Vector3 (nScale,nScale,nScale);
	}

	protected void hideHand()
	{
		tutorialHand.gameObject.SetActive (false);
	}

	protected void showHandAt(Vector3 pos)
	{
		tutorialHand.transform.position = pos;
	}

	protected void moveHandFromGameObjects(GameObject goFrom,GameObject goTo,float time = 1)
	{
		tutorialHand.transform.position = goFrom.transform.position;
		
		tutorialHand.transform.DOLocalMove (goTo.transform.position, time).OnComplete(()=>{if(OnMovementComplete!=null){OnMovementComplete();}});
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