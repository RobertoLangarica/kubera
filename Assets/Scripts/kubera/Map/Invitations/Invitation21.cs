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
	public Text atention;
	public Text atention1;

	void Start()
	{
		invitationManager = FindObjectOfType<InvitationToReview> ();

		first.SetActive (true);
		second.SetActive (false);

		firstText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_21_TEXT1).Replace ("{{n}}", System.Environment.NewLine);
		answerText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_21_ANSWER1);

		option1Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_21_OPTION1);
		option2Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_21_OPTION2);
		atention.text = atention1.text =  MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_TITLE_TEXT1);
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


