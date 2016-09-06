using UnityEngine;
using System;
using System.Collections;
using villavanilla.Notifications;

namespace Data
{
	[Serializable]
	public class NotificationJSONData : BasicData 
	{
		public string ID;
		public ERegisteredNotification templateName;
		public string toAppearDate;

		public NotificationJSONData(string nID,ERegisteredNotification type,string date)
		{
			ID = nID;
			templateName = type;
			toAppearDate = date;
		}

		public bool stillActive()
		{
			DateTime lastDate = DateTime.ParseExact (toAppearDate,"dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

			if (DateTime.Compare (lastDate, DateTime.Now) > 0) 
			{
				return true;
			}

			return false;
		}
	}
}
