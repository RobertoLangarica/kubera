using UnityEngine;
using System.Collections;

namespace Data.Sync
{
	public class GameUser
	{
		public string facebookId;
		public string customId;//Id de servicio custom
		public string id;//Id en backend de juego
		public bool newlyCreated;//Indica si es un usuario nuevo
	}	
}