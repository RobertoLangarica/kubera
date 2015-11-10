using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PopUp : MonoBehaviour 
{
	protected Vector3 initialPos;

	// Use this for initialization
	void Start () 
	{
		initialPos = new Vector3(Screen.width*0.5f,Screen.height*1.5f,0);
		transform.position = initialPos;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			showUp();
		}
		if(Input.GetKeyDown(KeyCode.S))
		{
			closePopUp();
		}
	}

	public void showUp()
	{
		transform.DOMove(new Vector3(Screen.width*0.5f,Screen.height*0.5f,0),1).SetEase(Ease.OutBack);
		//transform.position = ;
	}

	public void closePopUp()
	{
		transform.DOMove(new Vector3(Screen.width*0.5f,Screen.height*1.5f,0),1).SetEase(Ease.InBack);
	}
}