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

	public int timeForLifeInMinutes;
	public int maximumLifes;

	protected bool showTimer;
	protected int currentMinutes = 0;
	protected int currentSeconds = 60;
	protected float updateLifeTimer;

	protected string life1NotificationID;
	private DataManagerKubera dataManager;

	void Start()
	{
		showTimer = false;

		dataManager = (DataManagerKubera.GetInstance () as DataManagerKubera);

		if (PersistentData.GetInstance ().fromLevelBuilder) 
		{
			return;
		}

		if (currentUser.playerLifes < dataManager.initialLifes) 
		{
			if (currentUser.lifeTimerDate != "") 
			{
				updateLifesSinceLastPlay ();
			} 
			else 
			{
				setLifeDate ();
				updateLifesSinceLastPlay ();
			}
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
			takeALife ();
			Debug.Log ("Se quitan las vidas");
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
		
		if (tempUsr.playerLifes == dataManager.initialLifes) 
		{
			setLifeDate ();
		}

		giveLifesToUser(-1);

		//setLifeDate ();

		//LocalNotification*******De cuando se queda sin vidas y gana 1 vida
		if (tempUsr.playerLifes == 0) 
		{
			int currentLevel = tempUsr.levels.Count +1;

			life1NotificationID = (LocalNotificationManager.GetInstance () as LocalNotificationManager).modifyAndScheduleNotificationByName (
				villavanilla.Notifications.ERegisteredNotification.LIFE_1,
				MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NOTIFICATION_LIFE1).Replace ("{{level}}", currentLevel.ToString ()),
				"Kubera", timeForLifeInMinutes * 60.0);
		}
	}

	public void giveALife()
	{
		gotALife ();

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

			int missingLifes = dataManager.initialLifes - tempUsr.playerLifes;

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
			giveLifesToUser(dataManager.initialLifes);
		}
	}

	public double getTimeToWait()
	{
		if (currentUser.playerLifes >= dataManager.initialLifes) 
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
		return (double)(timeForLifeInMinutes * (dataManager.initialLifes - currentUser.playerLifes) * 60);
	}

	protected double lifeDateDifferenceInSecs()
	{
		DateTime lastDate = DateTime.ParseExact (currentUser.lifeTimerDate,"dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

		TimeSpan? span = DateTime.UtcNow - lastDate;

		return span.Value.TotalSeconds;
	}

	protected void decreaseLifeTimer(int seconds = 1)
	{
		if (currentUser.playerLifes == dataManager.initialLifes) 
		{
			showTimer = false;
			refreshHUD ();
			return;
		} 

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
		giveLifesToUser();

		if (currentUser.playerLifes == dataManager.initialLifes) 
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