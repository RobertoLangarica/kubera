using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Invitation32 : MonoBehaviour {

	public InvitationToReview invitationManager;
	public Text firstText;

	public Button option1;
	public Text option1Text;

	void Start()
	{
		invitationManager = FindObjectOfType<InvitationToReview> ();
		firstText.text = "¡Hola, qué bueno que llegaste, todos te están esperando! \nLa firma de autógrafos comenzará pront.";
		option1Text.text = "¡Excelente!";
	}

	public void close()
	{
		invitationManager.finish ();
	}

}


