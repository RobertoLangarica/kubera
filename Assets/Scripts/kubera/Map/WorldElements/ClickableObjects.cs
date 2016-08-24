using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class ClickableObjects : MonoBehaviour {

	protected Button button;
	protected Transform thisTransform;
	protected bool inAction;
	
	public float timeAction = 0.5f;
	public string fxName = "test";

	void Start()
	{
		thisTransform = transform;
		button = GetComponent<Button> ();
		if(button == null)
		{
			button = gameObject.AddComponent<Button> ();
		}

		button.transition = Selectable.Transition.None;
		button.onClick.AddListener(() => {
			doAction();
		});
	}

	public void doAction()
	{
		if(!inAction)
		{
			inAction = true;
			thisTransform.transform.DOShakeScale (timeAction).OnComplete(()=>{inAction= false;});
			AudioManager.GetInstance().Stop(fxName);
			AudioManager.GetInstance().Play(fxName);
		}
	}
}
