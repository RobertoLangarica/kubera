using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLeaderboard : MonoBehaviour {

	public string id;
	public GameObject slot;
	public Transform parent;

	protected List<RankSlot> slots = new List<RankSlot>();

	public List<RankSlot> getRankSlots()
	{
		return slots;
	}

	public void setSlotInfo(Sprite picture, string name, int score, int rank)
	{
		GameObject go;
		RankSlot rankSlot = null;
		go = Instantiate (slot) as GameObject;
		go.name = "slot";
		rankSlot = go.GetComponent<RankSlot> ();
		go.transform.SetParent(parent,false);
		go.SetActive (false);

		rankSlot.setPicture (picture);
		rankSlot.setName (name);
		rankSlot.setScore (score);
		rankSlot.setRank (rank);

		slots.Add (rankSlot);
	}

	public void showSlots(bool show)
	{
		for(int i=0; i<slots.Count; i++)
		{
			slots[i].gameObject.SetActive (show);
		}
	}

	public void deleteSlots()
	{
		for(int i=0; i<slots.Count; i++)
		{
			DestroyImmediate (slots [i].gameObject);
		}
	}

	public void moveSlots(Transform parent)
	{
		for(int i=0; i<slots.Count; i++)
		{
			slots [i].transform.SetParent (parent,false);
		}
	}
}
