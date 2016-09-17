using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Data
{
	[System.Serializable]
	public class MultipleNotificationUsers : BasicData 
	{
		public List<NotificationUser> users;

		public MultipleNotificationUsers()
		{
			users = new List<NotificationUser> ();
		}

		public NotificationUser getUserById(string id)
		{
			return users.Find(item=>item._id == id);
		}
	}
}