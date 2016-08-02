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
		welcomeText.text = "¡A Todos los visitantes de Kubera, les informamos que habrá una firma de autógrafos en la librería!";
	}

	public void close()
	{
		invitationManager.finish ();
	}

}


