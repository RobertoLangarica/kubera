using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LettersControllerAnimation : MonoBehaviour {

	protected Vector3 firstPosition;
	public Transform explodePosition;
	public RectTransform rectTransform;
	public Text letter;
	string[] letters = new string[] { "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z" };
	void Start()
	{
		this.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360)));
		firstPosition = this.transform.position;

		this.transform.position = new Vector3 (0, 0, 0);

		letter.text = letters [Random.Range (0, letters.Length - 1)];
	}

	public void firstPositionMove(float speed)
	{
		this.transform.DOMove (firstPosition, speed);
	}

	public void explode(float speed)
	{
		this.transform.DOMove (explodePosition.position, speed);
	}

	public void ultimatePosition(float speed)
	{
		rectTransform.DOAnchorPos (Vector2.zero, speed);
	}
}
