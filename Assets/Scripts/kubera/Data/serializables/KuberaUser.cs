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
		public int remoteDataVersion;//version de los datos del server
		public string facebookId;//Id de facebook del usuario
		public List<LevelData> levels;//niveles ya pasados
		public List<LevelData> levelsPlayed;//niveles ya pasados
		public int playerLifes;//Vidas del usuario
		public string lifeTimerDate;//Para dar vidas por tiempo
		public int maxLevelReached;//Para avance de mapa
		public bool firstTimeShopping;//Para emebeber video de shopika

		public KuberaUser()
		{
			levels = new List<LevelData>();
			firstTimeShopping = true;
		}

		public KuberaUser(string userId)
		{
			_id = userId;
			levels = new List<LevelData>();
			firstTimeShopping = true;
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			base.updateFrom (readOnlyRemote, ignoreVersion);

			LevelData level;

			//-1 es el dato vacio del server y se debe ignorar
			remoteDataVersion = ((KuberaUser)readOnlyRemote).remoteDataVersion < 0 ? this.remoteDataVersion:((KuberaUser)readOnlyRemote).remoteDataVersion;
				
			//Le quitamos lo sucio a los datos
			isDirty = false;

			//-1 es el dato vacio del server y se debe ignorar
			if(((KuberaUser)readOnlyRemote).maxLevelReached >= 0)
			{
				if(!upgradeMaxLevelReached(((KuberaUser)readOnlyRemote).maxLevelReached))
				{
					if(maxLevelReached > ((KuberaUser)readOnlyRemote).maxLevelReached)
					{
						Debug.Log("DIRTY FOR MAX REACHED: "+((KuberaUser)readOnlyRemote).maxLevelReached);
						//El local es mayor
						isDirty = true;
					}		
				}

			}

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

		public bool upgradeMaxLevelReached(int incommingValue)
		{
			bool updgraded = false;

			if(incommingValue > maxLevelReached)
			{
				maxLevelReached = incommingValue;
				updgraded = true;
			}

			return updgraded;
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
			maxLevelReached = 0;
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
			result.maxLevelReached = this.maxLevelReached;

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
				if(item.passed && item.world > result)
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

		public int countPassedLevelsByWorld(int world)
		{
			int result = 0;

			foreach(LevelData item in levels)
			{
				if(item.world == world && item.passed == true)
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
