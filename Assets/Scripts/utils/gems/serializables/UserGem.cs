using UnityEngine;
using System.Collections;
using Data;


namespace utils.gems
{
	public class UserGem:BasicData
	{
		public int gems;

		public override bool compareAndUpdate (BasicData remote, bool ignoreVersion = false)
		{
			//aqui no hay versiones, lo que viene del server es lo rifado
			int prevGems = gems;
			updateFrom(remote, ignoreVersion);

			if(prevGems != gems)
			{
				
			}
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			gems = ((UserGem)readOnlyRemote).gems;
		}
	}	
}