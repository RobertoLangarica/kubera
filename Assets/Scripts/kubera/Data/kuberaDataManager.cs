using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data;
using Kubera.Data.Sync;
using Kubera.Data.Remote.PFResponseData;

namespace Kubera.Data
{
	public class KuberaDataManager : LocalDataManager<MultipleUsers>
	{
		public KuberaSyncManger syncManager;
		public int initialLifes = 5;

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
			KuberaUser anonymous = new KuberaUser(ANONYMOUS_USER);
			anonymous.playerLifes = initialLifes;
			currentData.users.Add(anonymous);
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
				level.world		= levelsList.getLevelByName(levelName).world;
				level.isDirty	= true;

				currentUser.addLevel(level);
			}
				
			//Es sucio porque ya estaba sucio o por un cambio aqui
			currentUser.isDirty = currentUser.isDirty || level.isDirty;

			if(currentUser.isDirty)
			{
				saveLocalData(false);

				//Mandamos un usuario solo con este nivel
				KuberaUser user = new KuberaUser(currentUserId);
				user.addLevel(level);

				syncManager.updateData(user);	
			}
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
			Level lvl = levelsList.getLevelByName(levelName);
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
				level.locked	= false;

				level.world		= levelsList.getLevelByName(levelName).world;
				level.isDirty	= true;

				currentUser.addLevel(level);
			}

			//Cuidamos de no sobreescribir algun valor previo
			currentUser.isDirty = currentUser.isDirty || level.isDirty;

			if(currentUser.isDirty)
			{
				saveLocalData(false);

				//mandamos un usuario solo con este nivel
				KuberaUser user = new KuberaUser(currentUserId);
				user.addLevel(level);

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

		public void giveUserLifes(int amount = 1)
		{
			int totalLifes = currentUser.playerLifes + amount;

			if (totalLifes > initialLifes) 
			{
				currentUser.playerLifes = initialLifes;
			} 
			else if (totalLifes < 0) 
			{
				currentUser.playerLifes = 0;
			}
			else 
			{
				currentUser.playerLifes = totalLifes;
			}

			saveLocalData(false);
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
					//NO existe y creamos uno nuevo
					user = currentData.getUserById(ANONYMOUS_USER);

					user._id = facebookId;
					user.facebookId = facebookId;
					newId = facebookId;

					//Agregamos un nuevo usuario anonimo
					currentData.users.Add(new KuberaUser(ANONYMOUS_USER));
				}
				else
				{
					//Diff de los datos sin verificar version
					user = currentData.getUserByFacebookId(facebookId);
					//prevalece la version del usuario que no es anonimo
					currentUser.remoteDataVersion = user.remoteDataVersion;
					user.compareAndUpdate(currentUser, true);
					newId = user._id;
					//Limpiamos al usuario anonimo
					currentUser.clear();
				}
			}
			else
			{
				//El nuevo usuario existe?
				if(currentData.getUserByFacebookId(facebookId) == null)
				{
					//Se crea un nuevo usuario
					user = new KuberaUser(facebookId);
					user.facebookId = facebookId;
					user.playerLifes = initialLifes;

					newId = facebookId;

					currentData.users.Add(user);

				}
				else
				{
					//Hay un cambio de usuario sin consecuencias
					newId = currentData.getUserByFacebookId(facebookId)._id;
				}
			}

			currentUserId = newId;
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
					currentData.getUserById(ANONYMOUS_USER)._id = newUserId;

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
					KuberaUser newUser = new KuberaUser(newUserId);
					newUser.playerLifes = initialLifes;
					currentData.users.Add(newUser);
				}
				else
				{
					//Hay un cambio de usuario sin consecuencias
				}
			}

			base.changeCurrentuser (newUserId);

			//currentData.isDirty = currentUser.isDirty;
			saveLocalData(false);
		}
			

		public override void setUserAsAnonymous ()
		{
			base.setUserAsAnonymous ();
			//currentData.isDirty = currentUser.isDirty;
		}

		public void diffUser(KuberaUser remoteUser, bool ignoreVersion = false)
		{
			if(currentUserId != remoteUser._id)
			{
				Debug.Log("Se recibieron datos de otro usuario: "+currentUserId+","+ remoteUser._id);	
				return;
			}

			currentUser.compareAndUpdate(remoteUser, ignoreVersion);
			saveLocalData(false);
		}

		/**
		 * Los datos de este usuario que necesiten subirse
		 **/ 
		public KuberaUser getUserDirtyData()
		{
			KuberaUser user = currentUser;
			KuberaUser result = new KuberaUser(user._id);

			result.facebookId = user.facebookId;
			result.levels = user.getDirtyLevelsCopy();

			//Se envian como no sucios
			result.markAllLevelsAsNoDirty();

			return result;
		}
	}
}
