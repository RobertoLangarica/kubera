using UnityEngine;
using System.Collections;

public class Frame : MonoBehaviour 
{
	public GameObject top;
	public GameObject left;
	public GameObject right;
	public GameObject rightShadow;
	public GameObject rightTopShadow;
	public GameObject bottom;
	public GameObject bottonShadow;
	public GameObject bottonRightShadow;
	public GameObject bottonLeftShadow;
	public GameObject leftTop;
	public GameObject topRight;
	public GameObject rightBottom;
	public GameObject bottomLeft;

	public void instanceFrames(GameObject frame,Transform parent,int sortinOrder = 0)
	{
		GameObject frameInstance = null;
		frameInstance = GameObject.Instantiate(frame) as GameObject;
		frameInstance.GetComponent<SpriteRenderer> ().sortingLayerName = "Grid";
		frameInstance.GetComponent<SpriteRenderer> ().sortingOrder = sortinOrder;
		frameInstance.transform.SetParent(parent,false);
		frameInstance.SetActive (true);
	}

}

