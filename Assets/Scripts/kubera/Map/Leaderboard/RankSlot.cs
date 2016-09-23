using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankSlot : MonoBehaviour {

	public Image image;
	public Text nameText;
	public Text scoreText;
	public Text rankText;

	public string slotName;
	public int score;
	public int rank;

	public void setPicture(Sprite picture)
	{
		image.sprite = picture;
	}

	public void setName(string userName)
	{
		slotName = userName;
		nameText.text = userName;
	}

	public void setScore(int userScore)
	{
		score = userScore;
		scoreText.text = userScore.ToString();
	}

	public void setRank(int userRank)
	{
		rank = userRank;
		rankText.text = userRank.ToString();
	}
}
