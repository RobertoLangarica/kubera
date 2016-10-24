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

	public GameObject cellManager;

	public void instanceFrames(GameObject frame,Transform parent,int sortinOrder = 0)
	{
		GameObject frameInstance = null;
		frameInstance = GameObject.Instantiate(frame) as GameObject;
		frameInstance.GetComponent<SpriteRenderer> ().sortingLayerName = "Grid";
		frameInstance.GetComponent<SpriteRenderer> ().sortingOrder = sortinOrder;

		frameInstance.transform.SetParent(parent.transform,false);
		frameInstance.SetActive (true);
	}

	public void correctSize(float cellSize, Transform parent)
	{
		float fixedSize = cellSize - 1;

		if(fixedSize <0)
		{
			fixedSize *= -1;
		}

		parent.localScale = new Vector3 (parent.localScale.x + fixedSize, parent.localScale.y + fixedSize);
	}
}

