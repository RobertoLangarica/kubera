using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Data
{
	public class LevelsDataManager : LocalDataManager<UserLevels>
	{
		protected Levels levelsList;

		void Start()
		{
			levelsList = PersistentData.instance.levelsData;	
		}

		public void savePassedLevel(string levelName, int stars, int points)
		{
			LevelData level = currentData.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.updateOnlyIncrementalValues(stars, points);
			}
			else
			{
				level = new LevelData();
				level.id = levelName;
				level.points = points;
				level.stars = stars;
				level.isDirty = true;
				currentData.addLevel(level);
			}

			//Cuidamos de no sobreescribir al gun valor previo
			currentData.isDirty = currentData.isDirty || level.isDirty; 

			saveLocalData(false);
		}

		public bool isLevelPassed(string levelName)
		{
			return currentData.existLevel(levelName);
		}

		public bool isLevelBlocked(string levelName)
		{
			List<Level> world = levelsList.getLevelWorld(levelName);
			int index = getLevelWorldIndex(levelName,world);
			bool blocked;

			if(index == 0)
			{
				if(world[0].world == 0)
				{
					//no hay niveles anteriores
					blocked = false;
				}
				else
				{
					//El mundo anterior
					world = levelsList.getWorldByIndex(world[0].world-1);
					blocked = !isLevelPassed(world[world.Count-1].name);
				}
			}
			else
			{
				//Nivel anterior
				blocked = !isLevelPassed(world[index-1].name);
			}

			return blocked;
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
			LevelData level = currentData.getLevelById(levelName);

			if(level == null)
			{
				return 0;
			}

			return level.stars;
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
