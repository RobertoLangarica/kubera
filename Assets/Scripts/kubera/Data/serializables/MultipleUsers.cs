using System;
using System.Collections;
using System.Collections.Generic;
using Data;

namespace Kubera.Data
{
	[Serializable]
	public class MultipleUsers : BasicData 
	{
		public List<KuberaUser> users;

		public MultipleUsers()
		{
			users = new List<KuberaUser>();
		}

		public override void updateFrom (BasicData readOnlyRemote, bool ignoreVersion = false)
		{
			base.updateFrom (readOnlyRemote, ignoreVersion);

			KuberaUser user;

			//Le quitamos lo sucio a los datos
			isDirty = false;

			//revisamos los usuarios
			foreach(KuberaUser remoteUser in ((MultipleUsers)readOnlyRemote).users )
			{
				user = getUserById(remoteUser.id);
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

		public KuberaUser getUserById(string id)
		{
			return users.Find(item=>item.id == id);
		}

		public KuberaUser getUserByFacebookId(string id)
		{
			return users.Find(item=>item.facebookId == id);
		}
	}	
}