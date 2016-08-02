using UnityEngine;
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
		firstText.text = "La firma de autógrafos se levará a cabo en el nivel "+lvlFirm+". ¡Gracias por esperar!";
		option1Text.text = "¡Perfecto!";
	}

	public void close()
	{
		invitationManager.finish ();
	}

}


