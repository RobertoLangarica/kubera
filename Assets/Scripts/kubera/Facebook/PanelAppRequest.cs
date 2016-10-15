using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PanelAppRequest : MonoBehaviour {

	public enum ERequestState
	{
		KEY,LIFE,NONE
	}

	public enum EAction
	{
		SEND,ACCEPT
	}
		
	public Image imageRequested;
	public Text infoText;
	public Button button;
	public Text buttonText;
	public GameObject panelFriendsImage;
	public GameObject friendImage;

	[HideInInspector]
	public FacebookManager facebookManager;

	public bool full;
	public ERequestState stateRequested;
	public EAction actionChosed;

	public int maxFriends = 5;
	public GridLayoutGroup gridLayoutGroup;

	protected List<string> friendInfo = new List<string>();//0 name 1 id 2 requestId

	public Sprite[] image;

	public delegate void DOnDeleteRequest(string requestID);
	public delegate void DOnSendGift(bool life, List<string> friendsIDs);
	public delegate void DOnAcceptGift(bool life, int count);

	public DOnDeleteRequest OnDeleteRequest;
	public DOnSendGift OnSendGift;
	public DOnAcceptGift OnAcceptGift;

	void Start()
	{
		StartCoroutine (setSizeOfObject());
	}
	IEnumerator setSizeOfObject()
	{
		yield return new WaitForSeconds (0.1f);
		SizeOfObject ();
	}

	public void SizeOfObject()
	{
		float cellSize = ((panelFriendsImage.GetComponent<RectTransform> ().rect.width -gridLayoutGroup.padding.left)/maxFriends)-gridLayoutGroup.spacing.x;

		gridLayoutGroup.cellSize = new Vector2(cellSize,cellSize);
	}

	public void setParent(Transform parent, bool worldPositionStays)
	{
		transform.SetParent (parent,worldPositionStays);
	}

	public void selectRequestState(ERequestState requestState)
	{
		stateRequested = requestState;
	}

	public void selectImage()
	{
		switch(stateRequested)
		{
		case ERequestState.KEY:
			imageRequested.sprite = image [0];
			break;
		case ERequestState.LIFE:
			imageRequested.sprite = image [1];
			break;
		}
	}

	public void selectText()
	{
		if(friendInfo.Count == 1)
		{
			if(actionChosed == EAction.ACCEPT)
			{
				if(stateRequested == ERequestState.KEY)
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" ha enviado una llave!";
				}
				else
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" ha enviado una Vida!";
				}
			}
			else
			{
				if(stateRequested == ERequestState.KEY)
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" necesita una llave!";
				}
				else
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" necesita una Vida!";
				}
			}

		}
		else if(friendInfo.Count == 2)
		{
			if(actionChosed == EAction.ACCEPT)
			{
				if(stateRequested == ERequestState.KEY)
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" y "+friendInfo[1].Split('-')[0]+ " te enviaron llaves!";
				}
				else
				{					
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" y "+friendInfo[1].Split('-')[0]+ " te enviaron vidas!";
				}
			}
			else
			{
				if(stateRequested == ERequestState.KEY)
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" y "+friendInfo[1].Split('-')[0]+ " necesitan llaves!";

				}
				else
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" y "+friendInfo[1].Split('-')[0]+ " necesitan vidas!";
				}
			}
		}
		else
		{
			if(actionChosed == EAction.ACCEPT)
			{
				if(stateRequested == ERequestState.KEY)
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" y otros amigos te enviaron llaves!";
				}
				else
				{					
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" y otros amigos te enviaron vidas!";
				}
			}
			else
			{
				if(stateRequested == ERequestState.KEY)
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" y otros amigos necesitan llaves!";

				}
				else
				{
					infoText.text = "¡"+friendInfo[0].Split('-')[0]+" y otros amigos necesitan vidas!";
				}
			}
		}
	}

	public void selectAction(EAction action)
	{
		actionChosed = action;
	}

	public void selectTextButton()
	{
		switch(actionChosed)
		{
		case EAction.SEND:
			buttonText.text = "enviar";
			//enviar
			break;
		case EAction.ACCEPT:
			//aceptar
			buttonText.text = "aceptar";
			break;
		}
	}

	public void addIds(string ids)
	{
		friendInfo.Add (ids);
	}

	public void addFriendPicture (Texture texture)
	{
		GameObject go = Instantiate (friendImage)as GameObject;
		go.transform.SetParent (panelFriendsImage.transform, false);

		go.GetComponent<FriendImage> ().setFriendImage (texture);
	}

	public void doAction()
	{
		List<string> requestIDs = new List<string> ();

		for(int i=0; i<friendInfo.Count; i++)
		{
			requestIDs.Add (friendInfo [i].Split ('-') [2]);
		}

		if(actionChosed == EAction.ACCEPT)
		{
			if(stateRequested == ERequestState.KEY)
			{
				facebookManager.acceptGift (false, friendInfo.Count,this.gameObject, requestIDs,friendInfo [0].Split ('-') [3]);
			}
			else
			{
				facebookManager.acceptGift (true, friendInfo.Count,this.gameObject,requestIDs);
			}
		}
		else if(actionChosed == EAction.SEND)
		{
			List<string> friendsIDs = new List<string> ();

			for(int i=0; i<friendInfo.Count; i++)
			{
				friendsIDs.Add (friendInfo [i].Split ('-') [1]);
				requestIDs.Add (friendInfo [i].Split ('-') [2]);
			}

			if(stateRequested == ERequestState.KEY)
			{				
				facebookManager.sendGift (false, friendsIDs,this.gameObject,requestIDs,friendInfo [0].Split ('-') [3]);
			}
			else
			{
				facebookManager.sendGift (true, friendsIDs,this.gameObject,requestIDs);
			}
		}
	}
}
