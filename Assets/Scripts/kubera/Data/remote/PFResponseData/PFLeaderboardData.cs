using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kubera.Data.Remote.PFResponseData
{
	[Serializable]
	public class PFLeaderboardData
	{
		public List<PFLeaderboardEntry> Leaderboard;
		public string name;
	}

	[Serializable]
	public class PFLeaderboardEntry
	{
		public string PlayFabId;
		public string DisplayName;
		public int StatValue;
		public int Position;
	}
}