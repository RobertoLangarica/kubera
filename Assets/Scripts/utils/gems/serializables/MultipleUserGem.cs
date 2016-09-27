using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Data;

namespace utils.gems
{
	[Serializable]
	public class MultipleUserGem : BasicData 
	{
		public string lastUsedId;

		public List<UserGem> users;

		public MultipleUserGem()
		{
			users = new List<UserGem>();
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			base.updateFrom (readOnlyRemote, ignoreVersion);

			UserGem user;

			//Le quitamos lo sucio a los datos
			isDirty = false;

			//revisamos los usuarios
			foreach(UserGem remoteUser in ((MultipleUserGem)readOnlyRemote).users )
			{
				user = getUserById(remoteUser._id);
				if(user != null)
				{
					//Actualizamos el usuario existente
					user.compareAndUpdate(remoteUser, ignoreVersion);

					isDirty = isDirty || user.isDirty;
				}
				else
				{
					//nivel nuevo
					users.Add(remoteUser);
				}
			}
		}

		public UserGem getUserById(string id)
		{
			return users.Find(item=>item._id == id);
		}
	}
}