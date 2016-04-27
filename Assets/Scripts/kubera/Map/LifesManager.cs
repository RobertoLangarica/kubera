using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class LifesManager : MonoBehaviour 
{
	public Text lifesCount;
	public Text lifesTimer;
	public GameObject lifesPopUp;

	public int timeForLifeInMinutes;

	protected bool showTimer;
	protected int currentMinutes = 0;
	protected int currentSeconds = 60;
	protected float updateLifeTimer;


	void Start()
	{
		showTimer = false;

		if (UserDataManager.instance.playerLifes < UserDataManager.instance.maximumLifes) 
		{
			updateLifesSinceLastPlay ();
		}
	}

	void Update()
	{
		if (showTimer) 
		{
			if (updateLifeTimer >= 1) 
			{
				updateLifeTimer = 0;
				decreaseLifeTimer ();
			}
			updateLifeTimer += Time.deltaTime;
		}

		//Prueba para las vidas
		/*if (Input.GetKeyUp (KeyCode.A)) 
		{
			setLifeDate ();
			Debug.Log ("Se setea la fecha de la ultima vez que se sale del mapa");
		}
		if (Input.GetKeyUp (KeyCode.B)) 
		{
			UserDataManager.instance.giveLifeToPlayer (-3);
			Debug.Log ("Se quitan las vidas");
		}
		if (Input.GetKeyUp (KeyCode.C))
		{
			UserDataManager.instance.giveLifeToPlayer (UserDataManager.instance.maximumLifes);
			Debug.Log ("Se dan todas las vidas");
		}
		if (Input.GetKeyUp (KeyCode.D))
		{
			updateLifesSinceLastPlay ();
			Debug.Log ("Se inicia el update");
		}*/
	}

	public void takeALife()
	{
		if (UserDataManager.instance.playerLifes == UserDataManager.instance.maximumLifes) 
		{
			setLifeDate ();
		}

		UserDataManager.instance.giveLifeToPlayer (-1);

		//setLifeDate ();
	}

	protected void updateLifesSinceLastPlay()
	{
		double toWait = calculateTotalWaitingTime ();
		double sinceLastPlay = lifeDateDifferenceInSecs ();
		double difference = 0;
		int minutes = 0;
		int lifesGained = 0;

		if (toWait > sinceLastPlay) 
		{
			difference = toWait - sinceLastPlay;

			//Se calcula si se consigio alguna vida
			minutes = (int)sinceLastPlay / 60;
			lifesGained = (int)minutes / timeForLifeInMinutes;

			//Se entregan las vidas que se hayan juntado
			if (lifesGained > 0) 
			{
				Debug.Log (lifesGained);
				UserDataManager.instance.giveLifeToPlayer (lifesGained);

				updateDateOnData (lifesGained);
			}

			int missingLifes = UserDataManager.instance.maximumLifes - UserDataManager.instance.playerLifes;

			difference -= (missingLifes - 1) * (60 * timeForLifeInMinutes);

			//Se dejan solos los segundos
			minutes = (int)difference / 60;
			//Se calcula el tiempo actual
			currentMinutes = minutes;
			currentSeconds = ((int)difference - (minutes*60));

			refreshHUD ();

			showTimer = true;
		} 
		else 
		{
			UserDataManager.instance.giveLifeToPlayer (UserDataManager.instance.maximumLifes);
		}
	}

	protected double calculateTotalWaitingTime()
	{
		return (double)(timeForLifeInMinutes * (UserDataManager.instance.maximumLifes - UserDataManager.instance.playerLifes) * 60);
	}

	protected double lifeDateDifferenceInSecs()
	{
		DateTime lastDate = DateTime.ParseExact (UserDataManager.instance.lifeTimerDate,"dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

		TimeSpan? span = DateTime.UtcNow - lastDate;

		return span.Value.TotalSeconds;
	}

	protected void decreaseLifeTimer(int seconds = 1)
	{
		currentSeconds--;

		if (currentSeconds < 0) 
		{
			currentMinutes--;
			if (currentMinutes < 0 && currentSeconds < 0) 
			{
				currentMinutes = 0;
				gotALife ();
			} 
			else 
			{
				currentSeconds = 59;
			}
		}


		refreshHUD ();
	}

	protected void gotALife()
	{
		UserDataManager.instance.giveLifeToPlayer ();

		if (UserDataManager.instance.playerLifes == UserDataManager.instance.maximumLifes) 
		{
			showTimer = false;	
		} 
		else 
		{
			currentMinutes = timeForLifeInMinutes;
			currentSeconds = 0;

			updateDateOnData (1);
		}
	}

	protected void refreshHUD()
	{
		if (showTimer) 
		{
			lifesTimer.text = numberToTimeFormat (currentMinutes) + ":" + numberToTimeFormat (currentSeconds);
		} 
		else 
		{
			lifesTimer.text = "Lleno";
		}

		if (lifesCount != null) 
		{
			lifesCount.text = UserDataManager.instance.playerLifes.ToString ();
		}
	}

	protected void setLifeDate()
	{
		DateTime lastDate = DateTime.UtcNow;

		UserDataManager.instance.lifeTimerDate = lastDate.ToString ("dd-MM-yyyy HH:mm:ss");
	}

	protected void updateDateOnData(int lifesRegained)
	{
		DateTime lastDate = DateTime.ParseExact (UserDataManager.instance.lifeTimerDate,"dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

		lastDate = lastDate.AddSeconds (lifesRegained * timeForLifeInMinutes *60);

		UserDataManager.instance.lifeTimerDate = lastDate.ToString ("dd-MM-yyyy HH:mm:ss");
	}

	protected string numberToTimeFormat(int number)
	{
		string result = "0";

		if (number < 10) 
		{
			result += number.ToString ();
		} 
		else 
		{
			result = number.ToString ();
		}

		return result;
	}


}