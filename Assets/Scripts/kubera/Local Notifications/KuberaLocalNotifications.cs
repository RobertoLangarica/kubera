﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kubera.Data;
using Kubera.Data.Sync;

public class KuberaLocalNotifications : MonoBehaviour 
{
	protected bool deleted = false;
	protected bool scheduled = false;

	void Start()
	{
		if (!deleted) 
		{
			(LocalNotificationManager.GetInstance () as LocalNotificationManager).cancelScheduledTypeOfNotifications (
				villavanilla.Notifications.ERegisteredNotification.NO_PLAYING_GAMES);

			deleted = true;
		}
	}

	void OnApplicationPause( bool pauseStatus )
	{
		if (pauseStatus && !scheduled) 
		{
			setNoPlayingDaysNotifications ();

			setFullLifesNotification ();
		}
		else if(LocalNotificationManager.GetInstance().currentUserId != null && !deleted)
		{
			(LocalNotificationManager.GetInstance () as LocalNotificationManager).cancelScheduledTypeOfNotifications (
				villavanilla.Notifications.ERegisteredNotification.NO_PLAYING_GAMES);

			deleted = true;
		}
	}

	void OnApplicationQuit()
	{
		if (!scheduled) 
		{
			setNoPlayingDaysNotifications ();

			setFullLifesNotification ();
		}
	}

	protected void setNoPlayingDaysNotifications()
	{
		//86400 sconds per day

		//3 days
		List<WorldData> worldData = KuberaDataManager.GetCastedInstance<KuberaDataManager> ().currentUser.worlds;
		int currentLevel = 1;


		if (worldData.Count != 0) 
		{
			currentLevel = worldData [worldData.Count - 1].levels.Count;
			currentLevel++;				
		}

		(LocalNotificationManager.GetInstance () as LocalNotificationManager).modifyAndScheduleNotificationByName (
			villavanilla.Notifications.ERegisteredNotification.NO_PLAYING_GAMES,
			MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NOTIFICATION_3_DAYS).Replace ("{{level}}", currentLevel.ToString ()),
			"Kubera", 60);//259200);

		//7 days
		(LocalNotificationManager.GetInstance () as LocalNotificationManager).modifyAndScheduleNotificationByName (
			villavanilla.Notifications.ERegisteredNotification.NO_PLAYING_GAMES,
			MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NOTIFICATION_7_DAYS),
			"Kubera",604800);

		//14 days
		(LocalNotificationManager.GetInstance () as LocalNotificationManager).modifyAndScheduleNotificationByName (
			villavanilla.Notifications.ERegisteredNotification.NO_PLAYING_GAMES,
			MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NOTIFICATION_14_DAYS),
			"Kubera",1209600);
		
		//30 days
		(LocalNotificationManager.GetInstance () as LocalNotificationManager).modifyAndScheduleNotificationByName (
			villavanilla.Notifications.ERegisteredNotification.NO_PLAYING_GAMES,
			MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NOTIFICATION_30_DAYS),
			"Kubera",2592000);

		scheduled = true;
	}

	protected void setFullLifesNotification()
	{
		if (LifesManager.GetInstance () != null) 
		{
			if (LifesManager.GetInstance ().getTimeToWait () > 5) 
			{
				(LocalNotificationManager.GetInstance () as LocalNotificationManager).modifyAndScheduleNotificationByName (
					villavanilla.Notifications.ERegisteredNotification.FULL_LIFES,
					MultiLanguageTextManager.instance.getTextByID (MultiLanguageTextManager.NOTIFICATION_FULL_LIFES),
					"Kubera", LifesManager.GetInstance ().getTimeToWait ());
			}
		}
	}
}
