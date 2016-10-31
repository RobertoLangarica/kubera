using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScrollToSelected : MonoBehaviour 
{
	private ScrollRect sr;

	public void Start() 
	{
		if(!this.gameObject.activeInHierarchy)
		{
			return;	
		}

		sr = this.gameObject.GetComponent<ScrollRect>();
		Dropdown parent = this.transform.parent.gameObject.GetComponent<Dropdown>();

		sr.normalizedPosition = new Vector2(0f, 1-((float)parent.value)/(parent.options.Count-1));
	}
}