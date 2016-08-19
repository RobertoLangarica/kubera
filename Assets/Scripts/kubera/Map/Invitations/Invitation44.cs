using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Invitation44 : MonoBehaviour {

	public InvitationToReview invitationManager;
	public Text firstText;
	public Text secondText;

	public Button option1;
	public Button option2;
	public Text option1Text;
	public Text option2Text;

	void Start()
	{
		invitationManager = FindObjectOfType<InvitationToReview> ();
		firstText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_44_TEXT1);
		secondText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_44_TEXT2);
		option1Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_44_OPTION1);
		option2Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_44_OPTION2);
	}

	public void optionChoosed(int option)
	{
		switch (option) {
		case 0:
			invitationManager.finish ();
			break;
		case 1:
			//TODO mandarlo a la tienda para hacer la reseña
			break;
		}
	}

}


