using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class WorldsPopUp : PopUpBase {

	public GridLayoutGroup grid;
	public MapManager mapManager;
	public RectTransform mapButton;
	public RectTransform facebookMessagesButton;
	public RectTransform popUpRect;
	public ScrollRect scrollRect;
	public MiniWorld[] worlds;
	public Transform previousParent;
	public ScrollSnap scrollSnap;

	private bool initialized = false;
	private float openWidthPercent;
	private float closedBtnWidthPercent;

	[HideInInspector]public int indexToshowOnFirstOpen = 0;
	[HideInInspector]public bool firstTimeOpened = true;

	void Start()
	{
		if(!initialized)
		{
			initialize();
		}
	}

	public void initialize()
	{
		grid.cellSize = new Vector2 (Screen.width * 0.75f, Screen.height);
		grid.spacing = Vector2.zero;

		//Tamaños para cuando se abre el menu
		openWidthPercent = Screen.width * 0.85f;
		closedBtnWidthPercent = Screen.width * 0.78f;

		initialized = true;
	}

	public override void activate()
	{

		if(!initialized)
		{
			initialize();
		}

		mapManager.OnClosePopUp += exit;

		float delayTime = 0.5f;

		if(firstTimeOpened)
		{
			scrollSnap.scrollToChild(indexToshowOnFirstOpen, false);
			firstTimeOpened = false;
		}

		if (mapButton.parent != transform.parent) 
		{
			//Nos aseguramos de que este cerrado
			popUpRect.anchoredPosition = new Vector2 (-openWidthPercent, 0);

			popUp.SetActive (true);

			//Popup en posicion cero para que se abra
			popUpRect.DOAnchorPos(Vector2.zero, 0.5f, true);

			//boton en posicion abierta
			mapButton.SetParent (transform.parent);
			mapButton.DOAnchorPos(new Vector2 (openWidthPercent, 0), 0.5f, true);


			//Boton cerrado
			facebookMessagesButton.SetParent(transform.parent);
			facebookMessagesButton.DOAnchorPos (new Vector2 (closedBtnWidthPercent,0), 0.5f, true);

			//Este hijo hasta arriba
			transform.SetAsLastSibling();
		}  
		else 
		{
			//Abierto sin delay
			popUpRect.anchoredPosition = Vector2.zero;

			popUp.SetActive (true);

			//Boton abierto
			mapButton.DOAnchorPos (new Vector2 (openWidthPercent, 0), delayTime, true);
			//Boton cerrado
			facebookMessagesButton.DOAnchorPos (new Vector2 (closedBtnWidthPercent,0), delayTime, true);

			//Este hijo hasta arriba
			transform.SetAsLastSibling();
		}
	}

	public void goToWorld(int world)
	{
		mapManager.changeCurrentWorld (world, true, false);
		CompletePopUp ();
	}

	public void exit()
	{
		mapManager.OnClosePopUp -= exit;
		CompletePopUp ();
	}

	public void initializeMiniWorld(int world, bool unLocked, int starsObtained, int worldStars)
	{
		if(world >= worlds.Length)
		{
			return;
		}

		worlds [world].worldPopUp = this;
		if(unLocked)
		{
			worlds [world].setStars (starsObtained, worldStars);
		}
		else
		{
			worlds [world].blocked ();
		}
	}

	public void toMessages()
	{
		mapManager.OnClosePopUp -= exit;
		CompletePopUp ("toFacebookMessages",false);
	}

	protected void animateWorldsLights(bool animate)
	{
		for(int i=0; i<worlds.Length; i++)
		{
			if(animate)
			{
				worlds [i].animateLights ();
			}
			else
			{
				worlds [i].killAnimateLights ();
			}
		}
	}

	protected void CompletePopUp(string action= "",bool withAnim = true)
	{
		if (withAnim) 
		{
			popUpRect.DOAnchorPos (new Vector2 (-Screen.width * 0.85f, 0), 0.5f, true).OnComplete (() => {
				OnComplete (action);
			});

			mapButton.DOAnchorPos (Vector2.zero, 0.5f, true);
			mapButton.SetParent (previousParent);

			facebookMessagesButton.DOAnchorPos (Vector2.zero, 0.5f, true);
			facebookMessagesButton.SetParent (previousParent);
		} 
		else 
		{
			OnComplete (action);

			facebookMessagesButton.DOAnchorPos (new Vector2(Screen.width * 0.85f,0),0.5f,true);
		}
	}
}
