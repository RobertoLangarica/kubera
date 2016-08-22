using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Data
{
	public class LevelsDataManager : LocalDataManager<MultipleUsers>
	{
		protected Levels levelsList;

		protected override void Start ()
		{
			base.Start ();
			levelsList = PersistentData.GetInstance().levelsData;

			//El usuario anonimo esta vacio
			//currentData.getUserById(ANONYMOUS_USER).clear();
		}


		protected override void fillDefaultData ()
		{
			base.fillDefaultData ();

			//Usuario anonimo
			currentData.users.Add(new UserLevels(ANONYMOUS_USER));
		}

		public void savePassedLevel(string levelName, int stars, int points)
		{
			LevelData level = currentUserLevels.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.updateOnlyIncrementalValues(stars, points) || level.isDirty;
				level.isDirty = level.updatePassed(true) || level.isDirty ;
			}
			else
			{
				level = new LevelData(levelName);
				level.points	= points;
				level.stars		= stars;
				level.passed	= true;
				level.isDirty	= true;
				currentUserLevels.addLevel(level);
			}

			//Cuidamos de no sobreescribir al gun valor previo
			currentData.isDirty = currentData.isDirty || level.isDirty;

			//TODO: guardar en server
			saveLocalData(false);

			level = currentUserLevels.getLevelById(levelName);
		}

		public bool isLevelPassed(string levelName)
		{
			LevelData level = currentUserLevels.getLevelById(levelName);

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
			LevelData level =  currentUserLevels.getLevelById(levelName);

			if(level != null)
			{
				return level.locked;
			}

			return true;
		}

		public void unlockLevel(string levelName)
		{
			LevelData level =  currentUserLevels.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.isDirty || level.updateLocked(false);
			}
			else
			{
				level = new LevelData(levelName);
				level.locked = false;
				level.isDirty = true;
				currentUserLevels.addLevel(level);
			}

			//Cuidamos de no sobreescribir algun valor previo
			currentData.isDirty = currentData.isDirty || level.isDirty;

			//TODO: Guardar al servidor
			saveLocalData(false);
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
			LevelData level = currentUserLevels.getLevelById(levelName);

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
			LevelData level = currentUserLevels.getLevelById(levelName);

			if(level == null)
			{
				return 0;
			}

			return level.points;
		}

		public UserLevels currentUserLevels
		{
			get
			{
				return currentData.getUserById(currentUserId);
			}
		}

		public override void changeCurrentuser (string newUserId)
		{
			//Si es anoanimo hay que ver si los avances se guardan
			if(currentUserId == ANONYMOUS_USER)
			{
				//El nuevo usuario existe?
				if(currentData.getUserById(newUserId) == null)
				{
					//Este usuario toma los datos anonimos
					currentData.getUserById(ANONYMOUS_USER).id = newUserId;

					//Agregamos un nuevo usuario anonimo
					currentData.users.Add(new UserLevels(ANONYMOUS_USER));
					saveLocalData(false);
				}
			}

			//TODO: Detenemos las request del usuario anterior
			base.changeCurrentuser (newUserId);

			currentData.isDirty = currentUserLevels.isDirty;

			//TODO: Bajamos los datos y despues de bajar subimos lo necesario
		}
	}
}
