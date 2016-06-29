using System;
using System.Collections;
using System.Collections.Generic;

namespace Data
{
	[Serializable]
	public class MultipleUsers : BasicData 
	{
		public List<UserLevels> users;

		public MultipleUsers()
		{
			users = new List<UserLevels>();
		}

		public override void updateFrom (BasicData readOnlyRemote)
		{
			base.updateFrom (readOnlyRemote);

			UserLevels user;

			//Le quitamos lo sucio a los datos
			isDirty = false;

			//revisamos los usuarios
			foreach(UserLevels remoteUser in ((MultipleUsers)readOnlyRemote).users )
			{
				user = getUserById(remoteUser.id);
				if(user != null)
				{
					//Actualizamos el usuario existente
					user.compareAndUpdate(remoteUser);

					isDirty = isDirty || user.isDirty;
				}
				else
				{
					//nivel nuevo
					users.Add(remoteUser);
				}
			}
		}

		public UserLevels getUserById(string id)
		{
			return users.Find(item=>item.id == id);
		}
	}	
}