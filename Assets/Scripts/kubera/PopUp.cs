using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PopUp : MonoBehaviour 
{
	public delegate void redButtonDel();
	public delegate void greenButtonDel();

	public redButtonDel redBDelegate;
	public greenButtonDel greenBDelegate;

	protected Vector3 initialPos;

	// Use this for initialization
	void Start () 
	{
		initialPos = new Vector3(Screen.width*0.5f,Screen.height*1.5f,0);
		transform.position = initialPos;

		redBDelegate += foo;
		greenBDelegate += foo;

		GetComponent<Image>().enabled = false;
	}

	protected void foo(){}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void showUp()
	{
		transform.DOMove(new Vector3(Screen.width*0.5f,Screen.height*0.5f,0),1).SetEase(Ease.OutBack).OnComplete(()=>{
			GetComponent<Image>().enabled = true;});
		//transform.position = ;
	}

	public void closePopUp()
	{
		GetComponent<Image>().enabled = false;
		transform.DOMove(new Vector3(Screen.width*0.5f,Screen.height*1.5f,0),1).SetEase(Ease.InBack);
	}

	public void redButton()
	{
		redBDelegate();
	}

	public void greenButton()
	{
		greenBDelegate();
	}
}