using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data;
using Kubera.Data.Sync;

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
			LevelData level = currentUser.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.updateOnlyIncrementalValues(stars, points) || level.isDirty;
				level.isDirty = level.updatePassed(true) || level.isDirty;
			}
			else
			{
				level = new LevelData(levelName);
				level.points	= points;
				level.stars		= stars;
				level.passed	= true;
				level.isDirty	= true;
				currentUser.addLevel(level);
			}

			//Cuidamos de no sobreescribir algun valor previo
			currentData.isDirty = currentData.isDirty || level.isDirty;

			saveLocalData(false);

			//TODO: guardar en server
			//syncManager.updateData(data);

		}

		public bool isLevelPassed(string levelName)
		{
			LevelData level = currentUser.getLevelById(levelName);

			if(level != null)
			{
				return level.passed;
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
			LevelData level =  currentUser.getLevelById(levelName);

			if(level != null)
			{
				return level.locked;
			}

			return true;
		}

		public void unlockLevel(string levelName)
		{
			LevelData level =  currentUser.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.updateLocked(false) || level.isDirty;
			}
			else
			{
				level = new LevelData(levelName);
				level.locked = false;
				level.isDirty = true;
				currentUser.addLevel(level);
			}

			//Cuidamos de no sobreescribir algun valor previo
			currentData.isDirty = currentData.isDirty || level.isDirty;


			saveLocalData(false);

			//TODO: Guardar al servidor

			//syncManager.updateData(data);
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
			LevelData level = currentUser.getLevelById(levelName);

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
			LevelData level = currentUser.getLevelById(levelName);

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
			Debug.Log("Current user: "+currentUserId);
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
		public string getUserDirtyData()
		{
			KuberaUser user = currentUser;
			KuberaUser tempUser = new KuberaUser(user.id);

			if(!user.isDirty)
			{
				return "";
			}

			tempUser.facebookId = user.facebookId;
			tempUser.levels = user.getDirtyLevels();

			if(user.levels.Count == 0)
			{
				return "";
			}

			return JsonUtility.ToJson(tempUser);
		}
	}
}
