using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using VoxelBusters.NativePlugins;
using villavanilla.Notifications;

public class testLocalNotifications : MonoBehaviour 
{
	protected string lastNotification;
	public ERegisteredNotification test1;
	public ERegisteredNotification test2;

	public void setNotification1Minute()
	{
		(LocalNotificationManager.GetInstance () as LocalNotificationManager).scheduleNotificationByName (test1);
	}

	public void setNotification5Minute()
	{
		lastNotification = (LocalNotificationManager.GetInstance () as LocalNotificationManager).scheduleNotificationByName (test2);
	}

	public void cancelLastNotification()
	{
		(LocalNotificationManager.GetInstance() as LocalNotificationManager).cancelScheduledNotification(lastNotification);
	}

	public void cancelAll5MinsNotifications()
	{
		(LocalNotificationManager.GetInstance() as LocalNotificationManager).cancelScheduledTypeOfNotifications (test2);
	}

	public void cancelAllNotifications()
	{
		(LocalNotificationManager.GetInstance () as LocalNotificationManager).cancelAllNotificationsForCurrentUser ();
	}
}
