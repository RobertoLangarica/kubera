using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WildCardPowerUp : PowerupBase
{
	protected ABC.WordManager wordManager;

	void Start()
	{
		wordManager = FindObjectOfType<ABC.WordManager> ();
	}

	public override void activate ()
	{
		wordManager.addCharacter(wordManager.getWildcard("10"), null);
		wordManager.activateButtonOfWordsActions (true);

		OnComplete ();
	}
}
