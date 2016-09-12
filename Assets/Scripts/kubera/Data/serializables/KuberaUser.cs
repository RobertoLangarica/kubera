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
		public int remoteDataVersion;
		public string facebookId;

		public List<LevelData> levels;

		public int playerLifes;

		public string lifeTimerDate;

		public KuberaUser()
		{
			levels = new List<LevelData>();
		}

		public KuberaUser(string userId)
		{
			_id = userId;
			levels = new List<LevelData>();
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			base.updateFrom (readOnlyRemote, ignoreVersion);

			LevelData level;

			remoteDataVersion = ((KuberaUser)readOnlyRemote).remoteDataVersion;
				
			//Le quitamos lo sucio a los datos
			isDirty = false;

			//revisamos los niveles
			foreach(LevelData remoteLevel in ((KuberaUser)readOnlyRemote).levels )
			{
				level = getLevelById(remoteLevel._id);

				if(level != null)
				{
					//Actualizamos el nivel existente
					level.compareAndUpdate(remoteLevel, ignoreVersion);

					isDirty = isDirty || level.isDirty;
				}
				else
				{
					//nivel nuevo
					addLevel(remoteLevel);
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
			return levels.Find(item=>item._id == id);
		}

		public bool existLevel(string id)
		{
			return (levels.Find(item=>item._id == id) != null);
		}
		
		public void addLevel(LevelData item)
		{
			levels.Add(item);
		}

		public void markAllLevelsAsNoDirty()
		{
			foreach(LevelData item in levels)
			{
				item.isDirty = false;
			}	
		}

		public void clear()
		{
			levels.Clear();
			remoteDataVersion = 0;
			facebookId = "";
			isDirty = false;
		}
			
		public List<LevelData> getDirtyLevelsCopy()
		{
			List<LevelData> result = new List<LevelData>();

			foreach(LevelData level in levels)
			{
				if(level.isDirty)
				{
					result.Add(level.clone());
				}
			}

			return result;
		}

		public KuberaUser clone()
		{
			KuberaUser result = new KuberaUser(this._id);
			result.isDirty = this.isDirty;
			result.version = this.version;
			result.remoteDataVersion = this.remoteDataVersion;
			result.facebookId = this.facebookId;

			foreach(LevelData level in this.levels)
			{
				result.addLevel(level.clone());
			}

			return result;
		}

		public int maxWorldReached()
		{
			int result = 0;

			foreach(LevelData item in levels)
			{
				if(item.world > result)
				{
					result = item.world;
				}
			}

			return result;
		}
			
		public int countLevelsByWorld(int world)
		{
			int result = 0;

			foreach(LevelData item in levels)
			{
				if(item.world == world)
				{
					result++;
				}
			}

			return result;
		}

		public List<LevelData> getLevelsByWorld(int world)
		{
			List<LevelData> result = new List<LevelData>();

			foreach(LevelData item in levels)
			{
				if(item.world == world)
				{
					result.Add(item);
				}
			}

			return result;
		}
	}
}
