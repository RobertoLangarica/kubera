using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Invitation32 : MonoBehaviour {

	public InvitationToReview invitationManager;
	public Text firstText;

	public Button option1;
	public Text option1Text;
	public Text atention;

	void Start()
	{
		invitationManager = FindObjectOfType<InvitationToReview> ();
		firstText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_32_TEXT1).Replace ("{{n}}", System.Environment.NewLine);
		option1Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_32_OPTION1);
		atention.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_TITLE_TEXT1);
	}

	public void close()
	{
		invitationManager.finish ();
	}

}


