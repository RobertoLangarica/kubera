using UnityEngine;
using UnityEngine.SceneManagement;

public class ButonsFromEditor : MonoBehaviour
{
	public GameObject toBuilderButton;
	public GameObject resetSceneButton;


	void Start()
	{
		if (PersistentData.GetInstance().fromLevelBuilder) 
		{
			//PersistentData.GetInstance().fromLevelBuilder = false;
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
		PersistentData.GetInstance().fromGameToEdit = true;

		ScreenManager.GetInstance().GoToScene("LevelBuilder");
	}

	public void resetLevel()
	{
		PersistentData.GetInstance().fromLevelBuilder = true;
		SceneManager.LoadScene ("Game");
	}

}


