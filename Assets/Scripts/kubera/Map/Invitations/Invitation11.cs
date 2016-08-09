using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Invitation11 : MonoBehaviour {

	public InvitationToReview invitationManager;
	public Text welcomeText;

	public Button exit;

	void Start()
	{
		invitationManager = FindObjectOfType<InvitationToReview> ();
		welcomeText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_11_TEXT1);
	}

	public void close()
	{
		invitationManager.finish ();
	}

}


