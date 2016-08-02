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

		firstText.text = "¡Yo te conozco! \n ¿Eres tú quien dará auografos en la librería?";
		answerText.text = "Juraría que eres tú";

		option1Text.text = "En absoluto";
		option2Text.text = "¡Para nada!";
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


