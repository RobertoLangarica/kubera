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

		public List<WorldData> worlds;

		public KuberaUser()
		{
			worlds = new List<WorldData>();
		}

		public KuberaUser(string userId)
		{
			id = userId;
			worlds = new List<WorldData>();
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			base.updateFrom (readOnlyRemote, ignoreVersion);

			WorldData world;

			//Version de playfab
			PlayFab_dataVersion = ((KuberaUser)readOnlyRemote).PlayFab_dataVersion;

			//Le quitamos lo sucio a los datos
			isDirty = false;

			//revisamos los niveles
			foreach(WorldData remoteWorld in ((KuberaUser)readOnlyRemote).worlds)
			{
				world = getWorldById(remoteWorld.id);

				if(world != null)
				{
					//Actualizamos el mundo existente
					world.compareAndUpdate(remoteWorld, ignoreVersion);

					isDirty = isDirty || world.isDirty;
				}
				else
				{
					//nivel nuevo
					remoteWorld.markAllLevelsAsNoDirty();//nunca se marcan como sucios al llegar
					worlds.Add(remoteWorld);
				}		
			}

			//Alguno de los mundos sigue sucio
			if(!isDirty)
			{
				foreach(WorldData item in worlds)
				{
					if(item.isDirty)
					{
						isDirty = true;
						return;//Dejamos de iterar
					}
				}
			}
		}


		public WorldData getWorldById(string id)
		{
			return worlds.Find(item=>item.id == id);
		}

		public bool existWorld(string worldId)
		{
			return (worlds.Find(item=>item.id == worldId) != null);
		}
		
		public void addWorld(WorldData world)
		{
			worlds.Add(world);
		}

		public void markAllworldsAsNoDirty()
		{
			foreach(WorldData item in worlds)
			{
				item.markAllLevelsAsNoDirty();
			}	
		}

		public void clear()
		{
			worlds.Clear();
			PlayFab_dataVersion = 0;
			facebookId = "";
			isDirty = false;
		}
			
		public List<WorldData> getDirtyWorlds()
		{
			List<WorldData> result = new List<WorldData>();

			foreach(WorldData world in worlds)
			{
				if(world.isDirty)
				{
					result.Add(world.getOnlyDirtyCopy());
				}
			}

			return result;
		}

		public KuberaUser clone()
		{
			KuberaUser result = new KuberaUser(this.id);
			result.isDirty = this.isDirty;
			result.version = this.version;
			result.PlayFab_dataVersion = this.PlayFab_dataVersion;
			result.facebookId = this.facebookId;

			foreach(WorldData world in this.worlds)
			{
				result.addWorld(world.clone());
			}

			return result;
		}
	}
}
