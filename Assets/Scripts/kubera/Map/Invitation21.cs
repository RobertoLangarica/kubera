using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Invitation21 : MonoBehaviour {

	public InvitationToReview invitationManager;
	public GameObject first;
	public GameObject second;

	public Text firstText;
	public Text answerText;

	public Button option1;
	public Button option2;
	public Text option1Text;
	public Text option2Text;

	void Start()
	{
		invitationManager = FindObjectOfType<InvitationToReview> ();

		first.SetActive (true);
		second.SetActive (false);

		firstText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_21_TEXT1);
		answerText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_21_ANSWER1);

		option1Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_21_OPTION1);
		option2Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_21_OPTION2);
	}

	public void optionChoosed()
	{
		first.SetActive (false);
		second.SetActive (true);
	}

	public void close()
	{
		invitationManager.finish ();
	}

}


