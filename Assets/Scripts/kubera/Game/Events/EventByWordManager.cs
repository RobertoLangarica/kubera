using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventByWordManager : MonoBehaviour
{
	public List<WordEvent> wordEvents = new List<WordEvent>();

	public delegate void DEventFound(PowerupBase.EType type,int amount);
	public DEventFound OnEventFound;

	public void initializeEventsFromData(string csv)
	{
		string[] info;

		string[] eventInfo = csv.Split(',');

		for(int i =0; i<eventInfo.Length; i++)
		{
			info = eventInfo[i].Split('-');

			WordEvent wordEvent = new WordEvent();
			wordEvent.word = info [0];

			switch (info[1]) {
			case "bomb":
				wordEvent.type = PowerupBase.EType.BOMB;
				break;
			case "help":
				wordEvent.type = PowerupBase.EType.HINT_WORD;
				break;
			case "block":
				wordEvent.type = PowerupBase.EType.BLOCK;
				break;
			case "rotate":
				wordEvent.type = PowerupBase.EType.ROTATE;
				break;
			case "ray":
				wordEvent.type = PowerupBase.EType.DESTROY;
				break;
			case "wildcard":
				wordEvent.type = PowerupBase.EType.WILDCARD;
				break;
			}

			wordEvent.amount = int.Parse( info [2]);

			wordEvents.Add (wordEvent);
		}
	}

	public void existEventByWord(string retrievedWord)
	{
		for(int i=0; i<wordEvents.Count; i++)
		{
			if(wordEvents [i].word == retrievedWord)
			{
				OnEventFound (wordEvents [i].type, wordEvents [i].amount);
			}
		}
	}
}

