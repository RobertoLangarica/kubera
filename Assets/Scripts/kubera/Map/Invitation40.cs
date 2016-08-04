﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Invitation40 : MonoBehaviour {

	public InvitationToReview invitationManager;
	public Text firstText;

	public Button option1;
	public Text option1Text;
	public int lvlFirm;

	void Start()
	{
		invitationManager = FindObjectOfType<InvitationToReview> ();
		firstText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_40_TEXT1).Replace ("{{lvlSelected}}",lvlFirm.ToString());
		option1Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_40_OPTION1);
	}

	public void close()
	{
		invitationManager.finish ();
	}

}


