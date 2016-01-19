using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EditorManager : MonoBehaviour {

	public InputField inputStar1;
	public InputField inputStar2;
	public InputField inputStar3;
	public Text lblScore;


	public void onStarValueChange()
	{
		lblScore.text = (int.Parse(inputStar1.text) + int.Parse(inputStar2.text) + int.Parse(inputStar3.text)).ToString();
	}

}
