using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Data
{
	[Serializable]
	public class UserLevels : BasicData 
	{

		public List<LevelData> levels;

		public UserLevels()
		{
			levels = new List<LevelData>();
		}

		public override void updateFrom (BasicData readOnlyRemote)
		{
			base.updateFrom (readOnlyRemote);

			LevelData level;

			//Le quitamos lo sucio a los datos
			isDirty = false;

			//revisamos los niveles
			foreach(LevelData remoteLevel in ((UserLevels)readOnlyRemote).levels )
			{
				level = getLevelById(remoteLevel.id);
				if(level != null)
				{
					//Actualizamos el nivel existente
					level.compareAndUpdate(remoteLevel);

					isDirty = isDirty || level.isDirty;
				}
				else
				{
					//nivel nuevo
					levels.Add(remoteLevel);
				}
			}
		}


		public LevelData getLevelById(string id)
		{
			return levels.Find(item=>item.id == id);
		}

		public bool existLevel(string id)
		{
			return (levels.Find(item=>item.id == id) != null);
		}
			
		public void addLevel(LevelData level)
		{
			levels.Add(level);
		}
	}	
}
