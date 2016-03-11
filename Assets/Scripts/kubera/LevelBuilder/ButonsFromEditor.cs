using UnityEngine;
using UnityEngine.SceneManagement;

public class ButonsFromEditor : MonoBehaviour
{
	public GameObject toBuilderButton;
	public GameObject resetSceneButton;


	void Start()
	{
		if (PersistentData.instance.fromLevelBuilder) 
		{
			PersistentData.instance.fromLevelBuilder = false;
			resetSceneButton.SetActive (true);
			toBuilderButton.SetActive (true);
		}
		else
		{
			Destroy (toBuilderButton);
			Destroy (resetSceneButton);
			Destroy (gameObject);
		}
	}

	public void goBackToBuilder()
	{
		PersistentData.instance.fromGameToEdit = true;

		ScreenManager.instance.GoToScene("LevelBuilder");
	}

	public void resetLevel()
	{
		PersistentData.instance.fromLevelBuilder = true;
		SceneManager.LoadScene ("Game");
	}

}


