using UnityEngine;
using System;
using System.Collections;

namespace Data
{
	[Serializable]
	public class LevelData : BasicData 
	{
		public int stars;
		public int points;
		public bool unlocked;
		public bool bossReached;

		public override void updateFrom (BasicData readOnlyRemote)
		{
			base.updateFrom (readOnlyRemote);

			isDirty = updateOnlyIncrementalValues(((LevelData)readOnlyRemote).stars, ((LevelData)readOnlyRemote).points);
		}

		public bool updateOnlyIncrementalValues(int _stars, int _points)
		{
			bool updated = false;

			if(stars < _stars)
			{
				stars = _stars;
				updated = true;
			}

			if(points < _points)
			{
				points = _points;
				updated = true;	
			}

			return updated;
		}
	}
}