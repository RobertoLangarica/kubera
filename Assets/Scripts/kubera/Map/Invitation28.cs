using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Invitation28 : MonoBehaviour {

	public InvitationToReview invitationManager;
	public GameObject first;
	public GameObject secondOption1;
	public GameObject secondOption2;

	public Text firstText;
	public Text answerOption1Text;
	public Text answerOption2Text;

	public Button option1;
	public Button option2;
	public Text option1Text;
	public Text option2Text;

	void Start()
	{
		invitationManager = FindObjectOfType<InvitationToReview> ();

		first.SetActive (true);
		secondOption1.SetActive (false);
		secondOption2.SetActive (false);

		firstText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_28_TEXT1);
		answerOption1Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_28_ANSWER1);
		answerOption2Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_28_ANSWER2);

		option1Text.text =MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_28_OPTION1);
		option2Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_28_OPTION2);
	}

	public void optionChoosed(int option)
	{
		switch (option) {
		case 0:
			secondOption1.SetActive (true);
			break;
		case 1:
			secondOption2.SetActive (true);
			break;
		}
		first.SetActive (false);
	}

	public void close()
	{
		invitationManager.finish ();
	}

}


