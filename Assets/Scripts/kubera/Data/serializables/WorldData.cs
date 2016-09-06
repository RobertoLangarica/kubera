using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Data;

namespace Kubera.Data
{
	[Serializable]
	public class WorldData:BasicData
	{
		public List<LevelData> levels;

		public WorldData()
		{
			levels = new List<LevelData>();	
			isDirty = false;
		}

		public WorldData(string worldId)
		{
			id = worldId;
			levels = new List<LevelData>();	
			isDirty = false;
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			base.updateFrom (readOnlyRemote, ignoreVersion);

			LevelData level;

			//Le quitamos lo sucio a los datos
			isDirty = false;

			//revisamos los niveles
			foreach(LevelData remoteLevel in ((WorldData)readOnlyRemote).levels )
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

			//Alguno de los niveles sigue sucio
			if(!isDirty)
			{
				foreach(LevelData item in levels)
				{
					if(item.isDirty)
					{
						isDirty = true;
						return;//Dejamos de iterar
					}
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

		public WorldData getOnlyDirtyCopy()
		{
			WorldData result = new WorldData(this.id);

			foreach(LevelData level in levels)
			{
				if(level.isDirty)
				{
					result.addLevel(level.clone());
				}
			}

			return result;
		}

		public void markAllLevelsAsNoDirty()
		{
			foreach(LevelData level in levels)
			{
				level.isDirty = false;
			}	
		}

		public WorldData clone(bool addLevels = true)
		{
			WorldData result = new WorldData(this.id);
			result.version = this.version;
			result.isDirty = this.isDirty;

			if(addLevels)
			{
				foreach(LevelData level in levels)
				{
					result.addLevel(level.clone());
				}
			}

			return result;
		}
	}
}
