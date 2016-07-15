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

	private void HandleNotification(string message, Dictionary<string, object> additionalData, bool isActive) {
		print("GameControllerExample:HandleNotification:message" + message);

		// When isActive is true this means the user is currently in your game.
		// Use isActive and your own game logic so you don't interrupt the user with a popup or menu when they are in the middle of playing your game.
		if (additionalData != null) {
			if (additionalData.ContainsKey("discount")) {
				// Take user to your store.
			}
			else if (additionalData.ContainsKey("actionSelected")) {
				// actionSelected equals the id on the button the user pressed.
				// actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
			}
		}
	}

	public void sendPushMessage(string message, string title = "Kubera")
	{		
		OneSignal.GetIdsAvailable((userId, pushToken) => {
			if (pushToken != null) {
				// See http://documentation.onesignal.com/v2.0/docs/notifications-create-notification for a full list of options.
				// You can not use included_segments or any fields that require your OneSignal 'REST API Key' in your app for security reasons.
				// If you need to use your OneSignal 'REST API Key' you will need your own server where you can make this call.

				var notification = new Dictionary<string, object>();
				notification["contents"] = new Dictionary<string, string>() { {"en", "Test Message"} };
				notification["headings"] = new Dictionary<string, string>() { {"en", title} };
				// Send notification to this device.
				notification["include_player_ids"] = new List<string>() { userId };
				// Example of scheduling a notification in the future.
				notification["send_after"] = System.DateTime.Now.ToUniversalTime().AddSeconds(30).ToString("U");

				//extraMessage = "Posting test notification now.";
				OneSignal.PostNotification(notification, (responseSuccess) => {
					//extraMessage = "Notification posted successful! Delayed by about 30 secounds to give you time to press the home button to see a notification vs an in-app alert.\n" + Json.Serialize(responseSuccess);
				}, (responseFailure) => {
					//extraMessage = "Notification failed to post:\n" + Json.Serialize(responseFailure);
				});
			}
			else
			{
				Debug.LogWarning("ERROR: Device is not registered.");
			}
		});		
	}
}
