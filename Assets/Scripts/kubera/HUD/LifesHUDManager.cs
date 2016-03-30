using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class LifesHUDManager : MonoBehaviour 
{
	public Text lifesCount;
	public Text lifesTimer;

	public int timeForLifeInMinutes;

	protected bool showTimer;
	protected int currentMinutes;
	protected int currentSeconds;
	protected float updateLifeTimer;

	void Start()
	{
		showTimer = false;

		Debug.Log (UserDataManager.instance.playerLifes.ToString());

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

		if (Input.GetKeyUp (KeyCode.A)) 
		{
			setLifeDate ();
		}
	}

	protected void updateLifesSinceLastPlay()
	{
		double toWait = 240;//calculateTotalWaitingTime ();
		double sinceLastPlay = 220;//lifeDateDifferenceInSecs ();
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
			Debug.Log(lifesGained);
			//UserDataManager.instance.giveLifeToPlayer (lifesGained);

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
			//UserDataManager.instance.giveLifeToPlayer (UserDataManager.instance.maximumLifes);
		}
	}

	protected double calculateTotalWaitingTime()
	{
		return (double)(timeForLifeInMinutes * (UserDataManager.instance.maximumLifes - int.Parse(lifesCount.text)));
	}

	protected double lifeDateDifferenceInSecs()
	{
		DateTime lastDate = DateTime.ParseExact (UserDataManager.instance.lifeTimerDate,"dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

		TimeSpan? span = DateTime.Now - lastDate;

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
		Debug.Log ("Gano una vida");
		//UserDataManager.instance.giveLifeToPlayer ();

		if (UserDataManager.instance.playerLifes == UserDataManager.instance.maximumLifes) 
		{
			showTimer = false;	
		} 
		else 
		{
			currentMinutes = timeForLifeInMinutes;
			currentSeconds = 0;
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
		lifesCount.text = UserDataManager.instance.playerLifes.ToString();
	}

	protected void setLifeDate()
	{
		int seconds = (((timeForLifeInMinutes - 1) - currentMinutes) * 60) + (60 - currentSeconds);

		Debug.Log (seconds);

		DateTime lastDate = DateTime.UtcNow.AddSeconds(seconds);

		Debug.Log (DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss"));
		Debug.Log (lastDate.ToString("dd-MM-yyyy HH:mm:ss"));

		//UserDataManager.instance.lifeTimerDate = lastDate.ToString ("dd-MM-yyyy HH:mm:ss");
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