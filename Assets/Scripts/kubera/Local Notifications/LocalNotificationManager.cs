using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using Data;
using villavanilla.Notifications;

public class LocalNotificationManager : LocalDataManager<MultipleNotificationUsers>
{
	[Serializable]
	public class NotificationData
	{
		public ERegisteredNotification notificationName;

		public string alertBody;
		public double timeInSeconds;
		public eNotificationRepeatInterval repeatInterval;
		public string soundName;//Keep the files in Assets/PluginResources/Android or iOS or Common folder.

		public bool iosHasAction;
		public string iosAlertAction;

		public string androidContentTitle;
		public string androidTickerText;
		public string androidLargeIcon;//Keep the files in Assets/PluginResources/Android or Common folder.

		protected CrossPlatformNotification.AndroidSpecificProperties androidProperties;
		protected CrossPlatformNotification.iOSSpecificProperties iosProperties;

		public CrossPlatformNotification createNotification(string content, string title,double nTime = 0,Dictionary<string,object> notificationExtraInfo = null)
		{
			alertBody = content;
			androidContentTitle = androidTickerText = title;

			return createNotification (nTime, notificationExtraInfo);
		}	

		public CrossPlatformNotification createNotification(double nTime = 0,Dictionary<string,object> notificationExtraInfo = null)
		{
			if (nTime != 0) 
			{
				timeInSeconds = nTime;
			}

			CrossPlatformNotification result = new CrossPlatformNotification ();IDictionary userInfo = null;
			if(notificationExtraInfo == null)
			{
				userInfo = new Dictionary<string,object>();
			}
			else
			{
				userInfo = notificationExtraInfo;
			}

			userInfo.Add ("notificationName",notificationName);

			initializeIOSProperties ();
			initializeAndroidProperties ();

			result.AlertBody 		= alertBody;
			result.FireDate 		= System.DateTime.Now.AddSeconds(timeInSeconds);
			result.RepeatInterval 	= repeatInterval;
			result.SoundName 		= soundName;
			result.UserInfo 		= userInfo;

			result.iOSProperties	= iosProperties;
			result.AndroidProperties = androidProperties;

			return result;
		}

		protected void initializeIOSProperties()
		{
			CrossPlatformNotification.iOSSpecificProperties properties = new CrossPlatformNotification.iOSSpecificProperties ();

			properties.HasAction = iosHasAction;
			properties.AlertAction = iosAlertAction;

			iosProperties = properties;
		}

		protected void initializeAndroidProperties()
		{
			CrossPlatformNotification.AndroidSpecificProperties properties = new CrossPlatformNotification.AndroidSpecificProperties ();

			properties.ContentTitle = androidContentTitle;
			properties.TickerText = androidTickerText;
			properties.LargeIcon = androidLargeIcon;

			androidProperties = properties;
		}
	}

	public delegate void DNotificatonsCallBack(CrossPlatformNotification notification,ERegisteredNotification notificationName);

	public List<NotificationData> notificationTemplates = new List<NotificationData>();

	protected CrossPlatformNotification launchingNotification;
	protected DNotificatonsCallBack lauchedCallBacks;
	protected DNotificatonsCallBack receivedCallBacks;

	public NotificationUser currentUser
	{
		get
		{
			return currentData.getUserById (currentUserId);
		}
	}

	protected override void Start ()
	{
		base.Start ();

		clearNotifications ();

		cleanNotificationListByDate ();
	}

	protected override void fillDefaultData ()
	{
		base.fillDefaultData ();

		//Usuario anonimo
		currentData.users.Add(new NotificationUser(ANONYMOUS_USER));
	}

	protected void cleanNotificationListByDate()
	{
		if (currentUser.notifications.Count == 0) 
		{
			return;
		}

		for (int i = currentUser.notifications.Count - 1; i >= 0; i--) 
		{
			if (!currentUser.notifications [i].stillActive ()) 
			{
				currentUser.notifications.RemoveAt (i);
			}
		}

		saveLocalData ();
	}

	void OnEnable()
	{
		NotificationService.DidLaunchWithLocalNotificationEvent += onLaunchedWithLocalNotification;

		NotificationService.DidReceiveLocalNotificationEvent  += onReceivedLocalNotification;
	}

	protected void onLaunchedWithLocalNotification (CrossPlatformNotification notification)
	{
		launchingNotification = notification;

		if (lauchedCallBacks != null) 
		{
			lauchedCallBacks (launchingNotification,(ERegisteredNotification)launchingNotification.UserInfo["notificationName"]);
		}
	}

	protected void onReceivedLocalNotification(CrossPlatformNotification notification)
	{
		if (receivedCallBacks != null) 
		{
			receivedCallBacks (notification,(ERegisteredNotification)notification.UserInfo["notificationName"]);
		}
	}

	public bool launchedWithNotification(DNotificatonsCallBack callBack)
	{
		if (launchingNotification != null) 
		{
			callBack (launchingNotification,(ERegisteredNotification)launchingNotification.UserInfo["notificationName"]);
			return true;
		} 
		else 
		{
			lauchedCallBacks += callBack;
		}

		return false;
	}

	public void unregisterForLaunchedNotificcation(DNotificatonsCallBack callBack)
	{
		lauchedCallBacks -= callBack;
	}

	public void registerForANotification(DNotificatonsCallBack callBack)
	{
		receivedCallBacks += callBack;
	}

	public void unregisterForReceivedNotificcation(DNotificatonsCallBack callBack)
	{
		receivedCallBacks -= callBack;
	}

	public string modifyAndScheduleNotificationByName(ERegisteredNotification notification,string content,string title,double nTime = 0)
	{
		string nID = "";

		for (int i = 0; i < notificationTemplates.Count; i++) 
		{
			if (notificationTemplates [i].notificationName == notification) 
			{
				CrossPlatformNotification newNotification = notificationTemplates [i].createNotification (content,title,nTime);

				nID = scheduleLocalNotification (newNotification);

				currentUser.addNotification(new NotificationJSONData(nID,notification,newNotification.FireDate.ToString("dd-MM-yyyy HH:mm:ss")));
			}
		}

		saveLocalData ();

		return nID;
	}

	public string scheduleNotificationByName(ERegisteredNotification notification,double nTime = 0)
	{
		string nID = "";

		for (int i = 0; i < notificationTemplates.Count; i++) 
		{
			if (notificationTemplates [i].notificationName == notification) 
			{
				CrossPlatformNotification newNotification = notificationTemplates [i].createNotification (nTime);

				nID = scheduleLocalNotification (newNotification);

				currentUser.addNotification(new NotificationJSONData(nID,notification,newNotification.FireDate.ToString("dd-MM-yyyy HH:mm:ss")));
			}
		}

		saveLocalData ();

		return nID;
	}

	protected string scheduleLocalNotification(CrossPlatformNotification notification)
	{
		Debug.Log ("Enviando " + notification.GetNotificationID());
		return NPBinding.NotificationService.ScheduleLocalNotification (notification);
	}

	public void cancelScheduledTypeOfNotifications(ERegisteredNotification typeName)
	{
		if (currentData != null) 
		{
			for (int i = currentUser.notifications.Count - 1; i >= 0; i--) 
			{
				if (currentUser.notifications [i].templateName == typeName) 
				{
					cancelLocalNotification (currentUser.notifications [i]);
					currentUser.notifications.RemoveAt (i);
				}
			}

			saveLocalData ();
		}
	}

	public void cancelScheduledNotification(string notificationID)
	{
		for (int i = 0; i < currentUser.notifications.Count; i++) 
		{
			if (currentUser.notifications [i].ID == notificationID) 
			{
				cancelLocalNotification (currentUser.notifications[i]);
				currentUser.notifications.RemoveAt(i);
				break;
			}
		}

		saveLocalData ();
	}

	public void cancelAllNotificationsForCurrentUser()
	{
		for (int i = 0; i < currentUser.notifications.Count; i++) 
		{
			cancelLocalNotification (currentUser.notifications[i]);
		}

		currentUser.notifications.Clear ();

		saveLocalData ();
	}

	protected void cancelLocalNotification(NotificationJSONData notification)
	{
		NPBinding.NotificationService.CancelLocalNotification(notification.ID);
	}

	protected void cancelAllLocalNotifications()
	{
		NPBinding.NotificationService.CancelAllLocalNotification();
	}

	private void clearNotifications ()
	{
		NPBinding.NotificationService.ClearNotifications();
	}

	public void registerNotificationTypes(bool badges = true,bool sound = true,bool alert = true)
	{
		if (badges) 
		{
			NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Badge);
		}

		if (sound) 
		{
			NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Sound);
		}

		if (alert) 
		{
			NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Alert);
		}
	}

	public bool enabledNotificationTypes (out bool badges,out bool sound, out bool alert)
	{
		NotificationType notificationTypes =  NPBinding.NotificationService.EnabledNotificationTypes();
		badges = (notificationTypes & NotificationType.Badge) != 0;
		sound = (notificationTypes & NotificationType.Sound) != 0;
		alert = (notificationTypes & NotificationType.Alert) != 0;
		return badges || sound || alert;
	}
}