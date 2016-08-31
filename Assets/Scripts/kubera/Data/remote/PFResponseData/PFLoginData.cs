using UnityEngine;
using System;
using System.Collections;

namespace Kubera.Data.Remote.PFResponseData
{

	/**
	{
 	"SessionTicket": "4D2----8D11F4249A80000-7C64AB0A9F1D8D1A.CD803BF233CE76CC",
    "PlayFabId": "10931252888739651331",
    "NewlyCreated": false
	}
	**/
	[Serializable]
	public class PFLoginData
	{
		public string	SessionTicket;
		public string	PlayFabId;
		public bool		NewlyCreated;
	}
}