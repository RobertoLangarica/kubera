using UnityEngine;
using System;
using System.Collections;
using Data;


namespace utils.gems
{
	[Serializable]
	public class UserGem:BasicData
	{
		//TODO el usuario debe de guardar sus consumos que el server no responda como terminados (hay que ver como)
		public string accesToken;
		public string  shareCode;
		public int gems;

		public UserGem()
		{
			gems = 0;
		}

		public UserGem(string id)
		{
			_id = id;
			gems = 0;
		}

		public override bool compareAndUpdate (BasicData remote, bool ignoreVersion = false)
		{
			//aqui no hay versiones, lo que viene del server es lo rifado
			int prevGems = gems;
			updateFrom(remote, ignoreVersion);

			return prevGems != gems;
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			gems = ((UserGem)readOnlyRemote).gems;
		}

		public void clear()
		{
			gems = 0;
			accesToken = "";
		}
	}	
}