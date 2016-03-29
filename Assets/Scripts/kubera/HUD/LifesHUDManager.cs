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
	}

	protected void updateLifesSinceLastPlay()
	{
		double toWait = 345;//calculateTotalWaitingTime ();
		double sinceLastPlay = 10;//lifeDateDifferenceInSecs ();
		double difference = 0;
		int minutes = 0;
		int lifesGained = 0;

		if (toWait > sinceLastPlay) 
		{
			difference = toWait - sinceLastPlay;

			//Se calcula si se consigio alguna vida
			minutes = (int)difference / 60;
			lifesGained = (int)minutes / timeForLifeInMinutes;

			//Se entregan las vidas que se hayan juntado
			Debug.Log(lifesGained);
			//UserDataManager.instance.giveLifeToPlayer (lifesGained);

			//Se dejan solos los segundos
			difference -= minutes * 60;
			//Se calcula el tiempo actual
			currentMinutes = timeForLifeInMinutes - (minutes - (timeForLifeInMinutes * lifesGained));
			currentSeconds = 60 - ((int)difference);

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

		if (currentSeconds == 0) 
		{
			currentMinutes--;
			if (currentMinutes == 0 && currentSeconds == 0) 
			{
				currentMinutes = 0;
				gotALife ();
			}
			currentSeconds = 59;
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
	}

	protected void refreshHUD()
	{
		lifesTimer.text = currentMinutes.ToString() + ":" + currentSeconds.ToString();
		lifesCount.text = UserDataManager.instance.playerLifes.ToString();
	}

	protected void setLifeDate()
	{
		string dateNow = DateTime.UtcNow.ToString ("dd-MM-yyyy HH:mm:ss");

		int mins = int.Parse (dateNow.Substring(14,2));
		int secs = int.Parse(dateNow.Substring(17,2));

		DateTime lastDate = DateTime.ParseExact (dateNow.Substring (0, 14) + numberToTimeFormat(mins - currentMinutes) + ":" + numberToTimeFormat(secs - currentSeconds),
			"dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

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