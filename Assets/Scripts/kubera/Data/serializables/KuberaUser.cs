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

		public Dictionary<string,LevelData> levels;

		public int maximumLifes = 2;//Maximo de vida que puede tener el jugador
		public int playerLifes;

		public string lifeTimerDate;

		public KuberaUser()
		{
			levels = new Dictionary<string, LevelData>();
			playerLifes = maximumLifes;
		}

		public KuberaUser(string userId)
		{
			_id = userId;
			levels = new Dictionary<string, LevelData>();
			playerLifes = maximumLifes;
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			base.updateFrom (readOnlyRemote, ignoreVersion);

			LevelData level;

			PlayFab_dataVersion = ((KuberaUser)readOnlyRemote).PlayFab_dataVersion;
				
			//Le quitamos lo sucio a los datos
			isDirty = false;

			//revisamos los niveles
			foreach(KeyValuePair<string, LevelData> remoteItem in ((KuberaUser)readOnlyRemote).levels )
			{
				level = getLevelById(remoteItem.Value._id);

				if(level != null)
				{
					//Actualizamos el nivel existente
					level.compareAndUpdate(remoteItem.Value, ignoreVersion);

					isDirty = isDirty || level.isDirty;
				}
				else
				{
					//nivel nuevo
					addLevel(remoteItem.Value);
				}
			}

			//Alguno de los niveles sigue sucio
			if(!isDirty)
			{
				foreach(KeyValuePair<string, LevelData> item in levels)
				{
					if(item.Value.isDirty)
					{
						isDirty = true;
						return;//Dejamos de iterar
					}
				}
			}
		}


		public LevelData getLevelById(string id)
		{
			if(levels.ContainsKey(id))
			{
				return levels[id];
			}

			return null;
		}

		public bool existLevel(string id)
		{
			
			return levels.ContainsKey(id);
		}
		
		public void addLevel(LevelData item)
		{
			levels.Add(item._id,item);
		}

		public void markAllLevelsAsNoDirty()
		{
			foreach(KeyValuePair<string, LevelData> item in levels)
			{
				item.Value.isDirty = false;
			}	
		}

		public void clear()
		{
			levels.Clear();
			PlayFab_dataVersion = 0;
			facebookId = "";
			isDirty = false;
		}
			
		public Dictionary<string, LevelData> getDirtyLevelsCopy()
		{
			Dictionary<string, LevelData> result = new Dictionary<string, LevelData>();

			foreach(KeyValuePair<string,LevelData> level in levels)
			{
				if(level.Value.isDirty)
				{
					result.Add(level.Value._id, level.Value.clone());
				}
			}

			return result;
		}

		public KuberaUser clone()
		{
			KuberaUser result = new KuberaUser(this._id);
			result.isDirty = this.isDirty;
			result.version = this.version;
			result.PlayFab_dataVersion = this.PlayFab_dataVersion;
			result.facebookId = this.facebookId;

			foreach(KeyValuePair<string, LevelData> level in this.levels)
			{
				result.addLevel(level.Value.clone());
			}

			return result;
		}

		public void giveLifeToPlayer(int amount = 1)
		{
			int totalLifes = playerLifes + amount;

			if (totalLifes > maximumLifes) 
			{
				playerLifes = maximumLifes;
			} 
			else if (totalLifes < 0) 
			{
				playerLifes = 0;
			}
			else 
			{
				playerLifes = totalLifes;
			}
		}

		public int maxWorldReached()
		{
			int result = 0;

			foreach(KeyValuePair<string, LevelData> item in levels)
			{
				if(item.Value.world > result)
				{
					result = item.Value.world;
				}
			}

			return result;
		}
			
		public int countLevelsByWorld(int world)
		{
			int result = 0;

			foreach(KeyValuePair<string, LevelData> item in levels)
			{
				if(item.Value.world == world)
				{
					result++;
				}
			}

			return result;
		}

		public List<LevelData> getLevelsByWorld(int world)
		{
			List<LevelData> result = new List<LevelData>();

			foreach(KeyValuePair<string, LevelData> item in levels)
			{
				if(item.Value.world == world)
				{
					result.Add(item.Value);
				}
			}

			return result;
		}
	}
}
