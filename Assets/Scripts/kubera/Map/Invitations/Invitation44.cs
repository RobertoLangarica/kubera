using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using VoxelBusters.NativePlugins;

public class Invitation44 : MonoBehaviour {

	public InvitationToReview invitationManager;
	public Text firstText;
	public Text secondText;

	public Button option1;
	public Button option2;
	public Text option1Text;
	public Text option2Text;
	public Text atention;

	void Start()
	{
		firstText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_44_TEXT1).Replace ("{{n}}", System.Environment.NewLine);
		secondText.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_44_TEXT2);
		option1Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_44_OPTION1);
		option2Text.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_44_OPTION2);
		atention.text = MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.INVITATION_TITLE_TEXT1);
	}

	public void optionChoosed(int option)
	{
		switch (option) {
		case 0:
			exit ();
			break;
		case 1:
			//TODO mandarlo a la tienda para hacer la reseña
			NPBinding.Utility.OpenStoreLink (NPBinding.Utility.GetBundleIdentifier ());
			Invoke ("activeExit", 0.15f);
			break;
		}
	}

	protected void activeExit()
	{
		exit ();
	}

	public void exit()
	{
		invitationManager.finish ();
	}

}


