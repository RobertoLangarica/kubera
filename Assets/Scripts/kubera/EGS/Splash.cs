//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Splash : MonoBehaviour 
{
	public string nextScene;

	public bool splashScreen;

	protected float timer;

	void Update()
	{
		if (splashScreen) 
		{
			if (timer >= 3) 
			{
				SceneManager.LoadScene (nextScene);
			}

			timer += Time.deltaTime;
		}
	}

	public void PlayPressed()
	{
		SceneManager.LoadScene (nextScene);		
	}

	/*[MenuItem("Edit/ResetPlayerprefs")]
	public static void DeletePlayerPrefs()
	{
		PlayerPrefs.DeleteAll ();
	}*/
}
