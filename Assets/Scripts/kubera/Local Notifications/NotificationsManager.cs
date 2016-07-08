using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using OneSignalPush.MiniJSON;

public class NotificationsManager : MonoBehaviour {

	void Start()
	{
		// Should only be called once when your app is loaded.
		// OneSignal.Init(OneSignal_AppId, GoogleProjectNumber, NotificationReceivedHandler(optional));
		OneSignal.Init("54a71637-0755-4b10-9d21-f91659637a36", "28159365108", HandleNotification);

		// Shows a Native iOS/Android alert dialog when the user is in your app when a notification comes in.
		OneSignal.EnableInAppAlertNotification(true);
	}

	private static void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive) {
		print("GameControllerExample:HandleNotification:message" + message);
		extraMessage = "Notification opened with text: " + message;

		// When isActive is true this means the user is currently in your game.
		// Use isActive and your own game logic so you don't interrupt the user with a popup or menu when they are in the middle of playing your game.
		if (additionalData != null) {
			if (additionalData.ContainsKey("discount")) {
				extraMessage = (string)additionalData["discount"];
				// Take user to your store.
			}
			else if (additionalData.ContainsKey("actionSelected")) {
				// actionSelected equals the id on the button the user pressed.
				// actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
				extraMessage = "Pressed ButtonId: " + additionalData["actionSelected"];
			}
		}
	}
}
