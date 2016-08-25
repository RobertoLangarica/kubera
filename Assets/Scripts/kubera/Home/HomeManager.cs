using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kubera.Data;

public class HomeManager : MonoBehaviour {

	public List<PiecesControllAnimation> pieces;
	public LettersControllerAnimation[] letters;
	public AnimatedSprite homeAnimatedSprite;

	public Transform[] positions;
	protected float speed = .33f;

	protected List<PiecesControllAnimation> piecesMoved = new List<PiecesControllAnimation>();

	public RectTransform play;
	public RectTransform config;
	public RectTransform facebookLogin;
	public RectTransform facebookLogOut;
	public GameObject block;

	void Start()
	{
		for(int i=0, j=0; i<pieces.Count; j++)
		{
			piecesMoved.Add (pieces[Random.Range (0, pieces.Count)]);
			pieces.Remove (piecesMoved [j]);
			piecesMoved [j].setPosition (positions [j].position);
			piecesMoved [j].explodePosition = positions [j + 8].position;
			piecesMoved [j].startRotate (speed);
		}
		StartCoroutine (showLetters ());


	}

	IEnumerator showLetters()
	{
		yield return new WaitForSeconds (speed * 3);
		for(int i=0; i<letters.Length; i++)
		{
			letters [i].firstPositionMove (speed);
		}
		yield return new WaitForSeconds (speed);
		homeAnimatedSprite.autoUpdate = true;
		yield return new WaitUntil (() => homeAnimatedSprite.sequences [0].currentFrame == 19);
		for(int i=0; i<letters.Length; i++)
		{
			letters [i].explode (speed);
		}
		yield return new WaitForSeconds (speed);
		for(int i=0; i<letters.Length; i++)
		{
			letters [i].ultimatePosition (speed);
		}
		for(int i=0; i<piecesMoved.Count; i++)
		{
			piecesMoved [i].ultimatePosition (positions[i+16].position, speed);
		}
		yield return new WaitForSeconds (speed);
		config.DOAnchorPos (Vector2.zero, speed);
		yield return new WaitForSeconds (speed);
		play.DOAnchorPos (Vector2.zero, speed);
		yield return new WaitForSeconds (speed);
		facebookLogin.DOAnchorPos (Vector2.zero, speed);
		facebookLogOut.DOAnchorPos (Vector2.zero, speed);
		yield return new WaitForSeconds (speed);
		block.SetActive (false);
	}

	public void goToScene(string scene)
	{
		if(AudioManager.GetInstance())
		{
			
			AudioManager.GetInstance().Play("fxButton");
		}

		ScreenManager.instance.GoToScene (scene);
	}

	//HACK
	public void ereaseData()
	{
		LevelsDataManager.GetInstance ().deleteData ();
		if(AudioManager.GetInstance())
		{
			
			AudioManager.GetInstance().Play("fxButton");
		}
	}
}
