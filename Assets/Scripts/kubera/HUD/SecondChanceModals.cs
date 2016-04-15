using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SecondChanceModals: MonoBehaviour
{
	public RectTransform lettersBlock;
	public RectTransform PiecesBlock;

	public GameObject[] powerUpModals;

	public RectTransform piecesContainer;
	public RectTransform LettersContainer;

	void Start()
	{
		for(int i=0; i<powerUpModals.Length; i++)
		{
			powerUpModals [i].SetActive (false);
		}

		lettersBlock.gameObject.SetActive (false);

		PiecesBlock.gameObject.SetActive (false);

	}
		
	public void activateModals(bool activate)
	{
		for(int i=0; i<powerUpModals.Length; i++)
		{
			powerUpModals [i].SetActive (activate);
		}
		lettersBlock.gameObject.SetActive (activate);
		PiecesBlock.gameObject.SetActive (activate);
	}

}