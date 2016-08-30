using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data;
using Kubera.Data.Sync;
using Kubera.Data.Remote.PFResponseData;

namespace Kubera.Data
{
	public class LevelsDataManager : LocalDataManager<MultipleUsers>
	{
		public KuberaSyncManger syncManager;

		protected Levels levelsList;

		protected override void Start ()
		{
			base.Start ();
			levelsList = PersistentData.GetInstance().levelsData;
		}

		public KuberaUser currentUser{get{return currentData.getUserById(currentUserId);}}

		protected override void fillDefaultData ()
		{
			base.fillDefaultData ();

			//Usuario anonimo
			currentData.users.Add(new KuberaUser(ANONYMOUS_USER));
		}

		public void savePassedLevel(string levelName, int stars, int points)
		{
			Level lvl = levelsList.getLevelByName(levelName);
			WorldData world = getOrCreateWorldById(lvl.world.ToString());

			LevelData level = world.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.updateOnlyIncrementalValues(stars, points) || level.isDirty;
				level.isDirty = level.updatePassed(true) || level.isDirty;

				world.isDirty = world.isDirty || level.isDirty;
			}
			else
			{
				Debug.Log("Level null:");
				level = new LevelData(levelName);
				level.points	= points;
				level.stars		= stars;
				level.passed	= true;
				level.world		= lvl.world;

				level.isDirty	= true;
				world.isDirty	= true;

				world.addLevel(level);
			}

			//Cuidamos de no sobreescribir algun valor previo
			currentData.isDirty = currentData.isDirty || world.isDirty;

			if(currentData.isDirty)
			{
				saveLocalData(false);

				//mandamos un usuario solo con este nivel
				KuberaUser user = new KuberaUser(currentUserId);
				WorldData worldToSend = world.clone(false);
				worldToSend.addLevel(level);
				user.addWorld(worldToSend);

				Debug.Log("Nivel pasado");
				syncManager.updateData(user);	
			}
		}

		public WorldData getOrCreateWorldById(string id)
		{
			KuberaUser user = currentUser;
			WorldData result = user.getWorldById(id);

			if(result == null)
			{
				result = new WorldData(id);
				user.addWorld(result);
				saveLocalData(false);
			}

			return result;
		}

		public bool isLevelPassed(string levelName)
		{
			Level lvl = levelsList.getLevelByName(levelName);
			WorldData world = currentUser.getWorldById(lvl.world.ToString());

			if(world != null)
			{
				LevelData level = world.getLevelById(levelName);

				if(level != null)
				{
					return level.passed;
				}	
			}


			return false;
		}

		public bool isLevelReached(string levelName)
		{
			List<Level> world = levelsList.getLevelWorld(levelName);
			int index = getLevelWorldIndex(levelName,world);
			bool reached = false;

			if(index == 0)
			{
				if(world[0].world == 1)
				{
					//No hay nadie antes
					reached = true;
				}
				else
				{
					//El mundo anterior
					world = levelsList.getWorldByIndex(world[0].world-1);
					reached = isLevelPassed(world[world.Count-1].name);
				}
			}
			//else if(index > 1)
			else
			{
				//Ya se paso el nivel anterior
				reached = isLevelPassed(world[index-1].name);
			}

			return reached;
		}

		public bool isLevelBoss(string levelName)
		{
			return levelsList.getLevelByName(levelName).isBoss;
		}

		public bool isLevelLocked(string levelName)
		{
			Level lvl = levelsList.getLevelByName(levelName);
			WorldData world = currentUser.getWorldById(lvl.world.ToString());

			if(world != null)
			{
				LevelData level =  world.getLevelById(levelName);

				if(level != null)
				{
					return level.locked;
				}
			}

			return true;
		}

		public void unlockLevel(string levelName)
		{
			Level lvl = levelsList.getLevelByName(levelName);
			WorldData world = getOrCreateWorldById(lvl.world.ToString());

			LevelData level =  world.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.updateLocked(false) || level.isDirty;
				world.isDirty = world.isDirty || level.isDirty;
			}
			else
			{
				level = new LevelData(levelName);
				level.locked	= false;
				level.world		= lvl.world;
				level.isDirty	= true;
				world.isDirty	= true;

				world.addLevel(level);
			}

			//Cuidamos de no sobreescribir algun valor previo
			currentData.isDirty = currentData.isDirty || world.isDirty;

			if(currentData.isDirty)
			{
				saveLocalData(false);

				//mandamos un usuario solo con este nivel
				KuberaUser user = new KuberaUser(currentUserId);
				WorldData worldToSend = world.clone(false);
				worldToSend.addLevel(level);
				user.addWorld(worldToSend);

				Debug.Log("Unlock nivel");
				syncManager.updateData(user);
			}
		}


		public int getLevelWorldIndex(string levelName, List<Level> world)
		{
			Level level;

			for(int i = 0; i < world.Count; i++)
			{
				level = world[i];

				if(level.name.Equals(levelName))
				{
					return i;
				}
			}

			return -1;
		}

		public Level[] getLevelsOfWorld(int worldIndex)
		{
			List<Level> sameWorldLevels = new List<Level> ();

			for (int i = 0; i < levelsList.levels.Length; i++)
			{
				if (levelsList.levels [i].world == worldIndex)
				{
					sameWorldLevels.Add (levelsList.levels[i]);
				}
			}

			return sameWorldLevels.ToArray ();
		}

		public int getLevelStars(string levelName)
		{
			Level lvl = levelsList.getLevelByName(levelName);
			WorldData world = currentUser.getWorldById(lvl.world.ToString());

			if(world == null)
			{
				return 0;	
			}	

			LevelData level = world.getLevelById(levelName);

			if(level == null)
			{
				return 0;
			}

			return level.stars;
		}

		public int getAllEarnedStars()
		{
			int result = 0;

			for (int i = 0; i < levelsList.levels.Length; i++)
			{
				result += getLevelStars (levelsList.levels[i].name);
			}

			return result;
		}

		public int getLevelPoints(string levelName)
		{
			Level lvl = levelsList.getLevelByName(levelName);
			WorldData world = currentUser.getWorldById(lvl.world.ToString());

			if(world == null)
			{
				return 0;	
			}

			LevelData level = world.getLevelById(levelName);

			if(level == null)
			{
				return 0;
			}

			return level.points;
		}

		public void temporalUserChangeWithFacebookId(string facebookId)
		{
			string newId;
			KuberaUser user;

			if(currentUserId == ANONYMOUS_USER)
			{
				//El nuevo usuario existe?
				if(currentData.getUserByFacebookId(facebookId) == null)
				{
					//Este usuario toma los datos anonimos
					user = currentData.getUserById(ANONYMOUS_USER);

					user.id = facebookId;
					user.facebookId = facebookId;
					newId = facebookId;

					//Agregamos un nuevo usuario anonimo
					currentData.users.Add(new KuberaUser(ANONYMOUS_USER));
				}
				else
				{
					//Diff de los datos sin verificar version
					user = currentData.getUserByFacebookId(facebookId);
					user.compareAndUpdate(currentUser, true);
					newId = user.id;
					//Limpiamos al usuario anonimo
					currentUser.clear();
				}
			}
			else
			{
				//El nuevo usuario existe?
				if(currentData.getUserByFacebookId(facebookId) == null)
				{
					user = new KuberaUser(facebookId);
					user.facebookId = facebookId;
					newId = facebookId;
					currentData.users.Add(user);

				}
				else
				{
					//Hay un cambio de usuario sin consecuencias
					newId = currentData.getUserByFacebookId(facebookId).id;
				}
			}

			currentUserId = newId;
			currentData.isDirty = currentUser.isDirty;
			saveLocalData(false);
		}
			
		public override void changeCurrentuser (string newUserId)
		{
			if(currentUserId == newUserId)
			{
				//No hay cambios que hacer
				return;	
			}

			//Si es anonimo hay que ver si los avances se guardan
			if(currentUserId == ANONYMOUS_USER)
			{
				//El nuevo usuario existe?
				if(currentData.getUserById(newUserId) == null)
				{
					//Este usuario toma los datos anonimos
					currentData.getUserById(ANONYMOUS_USER).id = newUserId;

					//Agregamos un nuevo usuario anonimo
					currentData.users.Add(new KuberaUser(ANONYMOUS_USER));
				}
				else
				{
					//Diff de los datos sin verificar version
					currentData.getUserById(newUserId).compareAndUpdate(currentUser, true);
					//Limpiamos al usuario anonimo
					currentUser.clear();
				}
			}
			else
			{
				//El nuevo usuario existe?
				if(currentData.getUserById(newUserId) == null)
				{
					currentData.users.Add(new KuberaUser(newUserId));
				}
				else
				{
					//Hay un cambio de usuario sin consecuencias
				}
			}

			base.changeCurrentuser (newUserId);

			currentData.isDirty = currentUser.isDirty;
			saveLocalData(false);
		}
			

		public override void setUserAsAnonymous ()
		{
			base.setUserAsAnonymous ();
			currentData.isDirty = currentUser.isDirty;
		}

		public void diffUser(KuberaUser remoteUser, bool ignoreVersion = false)
		{
			if(currentUserId != remoteUser.id)
			{
				Debug.Log("Se recibieron datos de otro usuario: "+currentUserId+","+ remoteUser.id);	
				return;
			}

			currentUser.compareAndUpdate(remoteUser, ignoreVersion);
		}

		/**
		 * Los datos de este usuario que necesiten subirse
		 **/ 
		public KuberaUser getUserDirtyData()
		{
			KuberaUser user = currentUser;
			KuberaUser result = new KuberaUser(user.id);

			result.facebookId = user.facebookId;
			result.worlds = user.getDirtyWorlds();

			return result;
		}

		public string getCSVKeysToQuery()
		{
			//string result = "id,facebookId,version,DataVersion";
			string result = "version,DataVersion";
			//string result = "DataVersion";

			//Mundos
			foreach (int key in levelsList.worlds.Keys)
			{
				result += "," + "world_" + key.ToString();
			}

			return result;
		}
	}
}
