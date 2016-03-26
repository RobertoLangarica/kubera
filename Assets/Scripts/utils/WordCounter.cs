using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class WordCounter : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		TextAsset abc = Resources.Load("ABCData/WORDS_spanish") as TextAsset;
		Regex regex = new Regex(@"\r\n?|\n");
		string[] words = regex.Replace(abc.text,"\n").Split('\n');
		Dictionary<int, int> count = new Dictionary<int, int>();

		for(int i = 0; i < 30; i++)
		{
			count.Add(i, 0);
		}

		int c;
		foreach(string s in words)
		{
			c = count[s.Length];
			count[s.Length] = ++c;
		}

		float percent = 0;
		for(int i = 0; i < 30; i++)
		{
			percent += ((float)count[i]/(float)words.Length);
			Debug.Log(i.ToString("00")+":\t"+count[i].ToString("000")+"\t\t"
				+(percent*100.0f).ToString("00.00")+"%");
		}
	}
}
