using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class PopUp : MonoBehaviour 
{
	public delegate void redButtonDel();
	public delegate void greenButtonDel();
	public delegate void onShowComplete();
	public delegate void onCloseComplete();

	public redButtonDel redBDelegate;
	public greenButtonDel greenBDelegate;
	public onShowComplete showComplete;
	public onCloseComplete closeComplete;

	protected Vector3 initialPos;

	// Use this for initialization
	void Start () 
	{
		initialPos = new Vector3(Screen.width*0.5f,Screen.height*1.5f,0);
		transform.position = initialPos;

		redBDelegate += foo;
		greenBDelegate += foo;
		showComplete += foo;
		closeComplete += foo;

		GetComponent<Image>().enabled = false;
	}

	protected void foo(){}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void showUp()
	{
		transform.DOMove(new Vector3(Screen.width*0.5f,Screen.height*0.5f,0),0.5f).SetEase(Ease.OutBack).OnComplete(()=>{
			GetComponent<Image>().enabled = true;showComplete();});
		//transform.position = ;
	}

	public void closePopUp()
	{
		GetComponent<Image>().enabled = false;
		transform.DOMove(new Vector3(Screen.width*0.5f,Screen.height*1.5f,0),0.5f).SetEase(Ease.InBack).OnComplete(()=>{
			closeComplete();});
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