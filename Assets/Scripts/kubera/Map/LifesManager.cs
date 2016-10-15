using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Kubera.Data;
using Kubera.Data.Sync;

public class LifesManager : Manager<LifesManager> 
{
	public List<Text> lifesCount = new List<Text> ();
	public List<Text> lifesTimer = new List<Text> ();

	protected int timeForLifeInMinutes = 30;
	public int maximumLifes;

	protected bool showTimer;
	protected int currentMinutes = 0;
	protected int currentSeconds = 60;
	protected float updateLifeTimer;

	protected string life1NotificationID;
	private DataManagerKubera dataManager;

	void Start()
	{
		if(timeForLifeInMinutes != 30)
		{
			Debug.Log("<color=red>Modo test: VIDAS NO EN 30min</color>");
		}

		showTimer = false;

		dataManager = (DataManagerKubera.GetInstance () as DataManagerKubera);
		maximumLifes = dataManager.initialLifes;

		if (dataManager.currentUserId != dataManager.ANONYMOUS_USER) 
		{
			maximumLifes += 2;
		}
		else if(maximumLifes < currentUser.playerLifes)
		{
			currentUser.playerLifes = maximumLifes;
		}

		if (PersistentData.GetInstance ().fromLevelBuilder) 
		{
			return;
		}

		if (currentUser.playerLifes < maximumLifes) 
		{
			if (currentUser.lifeTimerDate == null) 
			{
				setLifeDate ();
			} 

			if (currentUser.lifeTimerDate == "") 
			{
				setLifeDate ();
			} 

			updateLifesSinceLastPlay ();
		}

		refreshHUD ();
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

		if (Input.GetKeyUp (KeyCode.B)) 
		{
			giveALife ();
			Debug.Log ("te dan una vida");
		}

		//Prueba para las vidas
		/*if (Input.GetKeyUp (KeyCode.A)) 
		{
			setLifeDate ();
			Debug.Log ("Se setea la fecha de la ultima vez que se sale del mapa");
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

	public KuberaUser currentUser{get{return dataManager.currentUser;}}

	public void takeALife()
	{
		KuberaUser tempUsr = currentUser;
		
		if (tempUsr.playerLifes == maximumLifes) 
		{
			setLifeDate ();
		}

		giveLifesToUser(-1);

		//setLifeDate ();

		//LocalNotification*******De cuando se queda sin vidas y gana 1 vida
		if (tempUsr.playerLifes == 0) 
		{
			int currentLevel = tempUsr.levels.Count;

			life1NotificationID = (LocalNotificationManager.GetInstance () as LocalNotificationManager).modifyAndScheduleNotificationByName (
				villavanilla.Notifications.ERegisteredNotification.LIFE_1,
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NOTIFICATION_LIFE1).Replace ("{{level}}", currentLevel.ToString ()),
				"Kubera", timeForLifeInMinutes * 60.0);
		}
	}

	public void giveALife(int amount = 1,bool fromGame = false)
	{
		gotALife (amount,fromGame);

		//CancelLocalNotification*******De cuando se queda sin vidas y gana 1 vida
		if (currentUser.playerLifes == 1) 
		{
			(LocalNotificationManager.GetInstance () as LocalNotificationManager).cancelScheduledNotification(life1NotificationID);
		}
	}

	protected void updateLifesSinceLastPlay()
	{
		double toWait = calculateTotalWaitingTime ();
		double sinceLastPlay = lifeDateDifferenceInSecs ();
		double difference = 0;
		int minutes = 0;
		int lifesGained = 0;
		KuberaUser tempUsr = currentUser;

		if (toWait > sinceLastPlay) 
		{
			difference = toWait - sinceLastPlay;

			//Se calcula si se consigio alguna vida
			minutes = (int)sinceLastPlay / 60;
			lifesGained = (int)minutes / timeForLifeInMinutes;

			//Se entregan las vidas que se hayan juntado
			if (lifesGained > 0) 
			{
				giveLifesToUser(lifesGained);

				updateDateOnData (lifesGained);
			}

			int missingLifes = maximumLifes - tempUsr.playerLifes;

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
			giveLifesToUser(maximumLifes);
		}
	}

	public double getTimeToWait()
	{
		if (currentUser.playerLifes >= maximumLifes) 
		{
			return 0;
		}

		double toWait = calculateTotalWaitingTime ();
		double sinceLastPlay = lifeDateDifferenceInSecs ();

		if (toWait > sinceLastPlay) 
		{
			return (toWait - sinceLastPlay);
		}
		else 
		{
			return 0;
		}
	}

	protected double calculateTotalWaitingTime()
	{
		return (double)(timeForLifeInMinutes * (maximumLifes - currentUser.playerLifes) * 60);
	}

	protected double lifeDateDifferenceInSecs()
	{
		DateTime lastDate = DateTime.ParseExact (currentUser.lifeTimerDate,"dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

		TimeSpan? span = DateTime.UtcNow - lastDate;

		return span.Value.TotalSeconds;
	}

	protected void decreaseLifeTimer(int seconds = 1)
	{
		if (currentUser.playerLifes == maximumLifes) 
		{
			showTimer = false;
			refreshHUD ();
			return;
		} 

		currentSeconds--;

		if (currentSeconds <= 0) 
		{
			currentMinutes--;
			if (currentMinutes < 0 && currentSeconds <= 0) 
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

	protected void gotALife(int amount = 1,bool fromGame = false)
	{
		giveLifesToUser(amount);

		if (currentUser.playerLifes == maximumLifes) 
		{
			showTimer = false;	
			refreshHUD ();
		} 
		else if(!fromGame)
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
			for (int i = 0; i < lifesTimer.Count; i++) 
			{
				lifesTimer[i].text = numberToTimeFormat (currentMinutes) + ":" + numberToTimeFormat (currentSeconds);
			}
		} 
		else 
		{
			for (int i = 0; i < lifesTimer.Count; i++) 
			{
				lifesTimer[i].text = MultiLanguageTextManager.instance.getTextByID(MultiLanguageTextManager.FULL_LIFES_TEXT);
			}
		}

		if (lifesCount != null) 
		{
			for (int i = 0; i < lifesCount.Count; i++) 
			{
				lifesCount[i].text = currentUser.playerLifes.ToString ();
			}
		}
	}

	protected void setLifeDate()
	{
		DateTime lastDate = DateTime.UtcNow;

		currentUser.lifeTimerDate = lastDate.ToString ("dd-MM-yyyy HH:mm:ss");
	}

	protected void updateDateOnData(int lifesRegained)
	{
		DateTime lastDate = DateTime.ParseExact (currentUser.lifeTimerDate,"dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

		lastDate = lastDate.AddSeconds (lifesRegained * timeForLifeInMinutes *60);

		currentUser.lifeTimerDate = lastDate.ToString ("dd-MM-yyyy HH:mm:ss");
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

	public void giveLifesToUser(int amount = 1)
	{
		(DataManagerKubera.GetInstance () as DataManagerKubera).giveUserLifes (amount);
	}
}