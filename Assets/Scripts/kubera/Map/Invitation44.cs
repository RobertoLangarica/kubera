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
		firstText.text = "¡Hola! Somos los creadores de Kubera.\n¿Nos darías tu autógrafo a maera de reseña?";
		secondText.text = "Nos encantaría leer lo que tienes que decirnos sobre kubera";
		option1Text.text = "¡Gracias, paso!";
		option2Text.text = "¡Claro que si!";
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


