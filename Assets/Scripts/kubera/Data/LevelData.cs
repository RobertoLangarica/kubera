using UnityEngine;
using System;
using System.Collections;
using Data;

namespace Kubera.Data
{
	[Serializable]
	public class LevelData : BasicData 
	{
		public int stars;
		public int points;
		public bool locked;
		public bool passed;
		public int world;

		public LevelData(string levelId)
		{
			id = levelId;
			locked = true;
			passed = false;
			stars = 0;
			points = 0;
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			base.updateFrom (readOnlyRemote, ignoreVersion);

			isDirty = false;

			if(!updateOnlyIncrementalValues(((LevelData)readOnlyRemote).stars, ((LevelData)readOnlyRemote).points))
			{
				if(stars > ((LevelData)readOnlyRemote).stars || points > ((LevelData)readOnlyRemote).points)
				{
					//Los puntos locales son mayores
					isDirty = true;
				}
			}

			if(!updateLocked(((LevelData)readOnlyRemote).locked))
			{
				if(!locked && ((LevelData)readOnlyRemote).locked)
				{
					//Ya esta desbloqueado pero del server no llego asi
					isDirty = true;
				}
			}

			if(!updatePassed(((LevelData)readOnlyRemote).passed))
			{
				if(passed && !((LevelData)readOnlyRemote).passed)
				{
					//Ya esta pasado pero del server no llego asi
					isDirty = true;
				}
			}
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

		public bool updatePassed(bool _passed)
		{
			bool updated = false;

			if(_passed && !passed)
			{
				passed = _passed;
				updated = true;
			}

			return updated;
		}


		public bool updateLocked(bool _locked)
		{
			bool updated = false;

			if(!_locked && locked)
			{
				locked = _locked;
				updated = true;
			}

			return updated;
		}
	}
}