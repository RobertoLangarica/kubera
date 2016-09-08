using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Data
{
	[System.Serializable]
	public class NotificationUser : BasicData 
	{
		public List<NotificationJSONData> notifications;

		public NotificationUser()
		{
			notifications = new List<NotificationJSONData> ();
		}

		public NotificationUser(string userID)
		{
			_id = userID;
			notifications = new List<NotificationJSONData> ();
		}

		public NotificationJSONData getNotificationById(string id)
		{
			return notifications.Find(item=>item._id == id);
		}

		public bool existNotification(string id)
		{
			return (notifications.Find(item=>item._id == id) != null);
		}

		public void addNotification(NotificationJSONData notification)
		{
			notifications.Add(notification);
		}

		public void clear()
		{
			notifications.Clear();
		}
	}
}