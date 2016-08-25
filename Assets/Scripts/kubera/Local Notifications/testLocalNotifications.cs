using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using VoxelBusters.NativePlugins;

public class testLocalNotifications : MonoBehaviour {

	//Create an instance of CrossPlatformNotification and fill with details.
	private CrossPlatformNotification CreateNotification (long _fireAfterSec, eNotificationRepeatInterval _repeatInterval)
	{
		// User info
		IDictionary _userInfo			= new Dictionary<string, string>();
		_userInfo["data"]				= "add what is required";

		CrossPlatformNotification.iOSSpecificProperties _iosProperties			= new CrossPlatformNotification.iOSSpecificProperties();
		_iosProperties.HasAction		= true;
		_iosProperties.AlertAction		= "alert action";


		CrossPlatformNotification.AndroidSpecificProperties _androidProperties	= new CrossPlatformNotification.AndroidSpecificProperties();
		_androidProperties.ContentTitle	= "content title";
		_androidProperties.TickerText	= "ticker ticks over here";
		_androidProperties.LargeIcon	= "NativePlugins.png"; //Keep the files in Assets/StreamingAssets/VoxelBusters/NativePlugins/Android folder.

		CrossPlatformNotification _notification	= new CrossPlatformNotification();
		_notification.AlertBody			= "alert body"; //On Android, this is considered as ContentText
		_notification.FireDate			= System.DateTime.Now.AddSeconds(_fireAfterSec);
		_notification.RepeatInterval	= _repeatInterval;
		_notification.UserInfo			= _userInfo;
		_notification.SoundName			= "Notification.mp3"; //Keep the files in Assets/StreamingAssets/VoxelBusters/NativePlugins/Android or iOS or Common folder.

		_notification.iOSProperties		= _iosProperties;
		_notification.AndroidProperties	= _androidProperties;

		return _notification;
	}

	public void go()
	{		
		//Schedule this local notification.
		CrossPlatformNotification _notification = CreateNotification(5, eNotificationRepeatInterval.MINUTE);
		NPBinding.NotificationService.ScheduleLocalNotification(_notification);
	}
}
