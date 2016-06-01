using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Data
{
	public class LevelsDataManager : LocalDataManager<UserLevels>
	{
		protected Levels levelsList;

		protected override void Start ()
		{
			base.Start ();
			levelsList = PersistentData.instance.levelsData;	
		}

		public void savePassedLevel(string levelName, int stars, int points)
		{
			LevelData level = currentData.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.isDirty || level.updateOnlyIncrementalValues(stars, points);
				level.isDirty = level.isDirty || level.updatePassed(true);
			}
			else
			{
				level = new LevelData(levelName);
				level.points	= points;
				level.stars		= stars;
				level.passed	= true;
				level.isDirty	= true;
				currentData.addLevel(level);
			}

			//Cuidamos de no sobreescribir al gun valor previo
			currentData.isDirty = currentData.isDirty || level.isDirty; 

			saveLocalData(false);
		}

		public bool isLevelPassed(string levelName)
		{
			LevelData level = currentData.getLevelById(levelName);

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
				if(world[0].world == 0)
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
			LevelData level =  currentData.getLevelById(levelName);

			if(level != null)
			{
				return level.locked;
			}

			return true;
		}

		public void unlockLevel(string levelName)
		{
			LevelData level =  currentData.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.isDirty || level.updateLocked(false);
			}
			else
			{
				level = new LevelData(levelName);
				level.locked = false;
				level.isDirty = true;
				currentData.addLevel(level);
			}

			//Cuidamos de no sobreescribir al gun valor previo
			currentData.isDirty = currentData.isDirty || level.isDirty; 

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

		public Level[] getLevesOfWorld(int worldIndex)
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
			LevelData level = currentData.getLevelById(levelName);

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
			LevelData level = currentData.getLevelById(levelName);

			if(level == null)
			{
				return 0;
			}

			return level.points;
		}
	}
}
