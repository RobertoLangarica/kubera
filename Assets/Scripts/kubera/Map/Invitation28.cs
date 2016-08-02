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

		firstText.text = "¡Naaaaaaaahhh si eres tú!";
		answerOption1Text.text = "Claro, claro. \nNo eres tú. \nNos vemos en la librería, guiño, guiño";
		answerOption2Text.text = "Tú secreto está a salvo conmigo. \nNos vemos en la librería. \n¡Muero por tu autógrafo!";

		option1Text.text = "¡No, no soy!";
		option2Text.text = "¡Ok, si soy yo!";
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


