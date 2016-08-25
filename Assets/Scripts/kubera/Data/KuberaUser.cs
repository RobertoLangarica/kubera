using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Data;

namespace Kubera.Data
{
	[Serializable]
	public class KuberaUser : BasicData 
	{
		public int PlayFab_dataVersion;
		public string facebookId;

		public List<LevelData> levels;

		public KuberaUser()
		{
			levels = new List<LevelData>();
		}

		public KuberaUser(string userId)
		{
			id = userId;
			levels = new List<LevelData>();
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			base.updateFrom (readOnlyRemote, ignoreVersion);

			LevelData level;

			//Version de playfab
			PlayFab_dataVersion = ((KuberaUser)readOnlyRemote).PlayFab_dataVersion;

			//Le quitamos lo sucio a los datos
			isDirty = false;

			//revisamos los niveles
			foreach(LevelData remoteLevel in ((KuberaUser)readOnlyRemote).levels )
			{
				level = getLevelById(remoteLevel.id);
				if(level != null)
				{
					//Actualizamos el nivel existente
					level.compareAndUpdate(remoteLevel, ignoreVersion);

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

		public void clear()
		{
			levels.Clear();
			isDirty = false;
		}

		public string getCSVKeysToQuery()
		{
			return "levels";
		}

		public List<LevelData> getDirtyLevels()
		{
			List<LevelData> result = new List<LevelData>();

			foreach(LevelData level in levels)
			{
				if(level.isDirty)
				{
					result.Add(level);
				}
			}

			return result;
		}
	}	
}
