using UnityEngine;
using System;
using System.Text;
using System.Collections;
using Data.Sync;

namespace Kubera.Data.Sync
{
	public class KuberaSyncManger : SyncManager
	{
		public KuberaDataManager localData;


		/**
		 * Login en local para que el usuario pueda comenzar a jugar aun cuando no se a hecho un login al servicio remoto 
		 **/
		protected override void prepareLocalLogin (string customUserId, string facebookUserId)
		{
			localData.temporalUserChangeWithFacebookId(facebookUserId);
		}

		/**
		 * Cuando se hace logout en local hay responder a ese cambio y hay que hacer logout remoto
		 **/ 
		protected override void logout ()
		{
			base.logout ();
			localData.setUserAsAnonymous();

			if(_mustShowDebugInfo)
			{
				Debug.Log("Logged OUT");
			}
		}

		/**
		 * Cuando se recibe el usuario (sin informacion solo sus identificaciones)
		 **/ 
		protected override void OnUserReceived (GameUser user)
		{
			base.OnUserReceived (user);

			//Exsitia un usuario creado con id de facebook temporal?
			KuberaUser kUser = localData.getCurrentData().getUserById(currentUser.facebookId);
			if(kUser != null)
			{
				//Le asignamos su id de verdad
				kUser.id = currentUser.id;
			}

			localData.changeCurrentuser(currentUser.id);

			//En caso de que se creo un nuevo usuario se le asigna su facebookId
			if(localData.currentUser.facebookId != currentUser.facebookId)
			{
				localData.currentUser.facebookId = currentUser.facebookId;
				localData.saveLocalData(false);
			}

			if(user.newlyCreated)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Creating remote user.");
				}

				//Datos nuevos DEPRECATED
				//server.createUserData<KuberaUser>(currentUser.id, userToPlayFabJSON(localData.currentUser), localData.currentUser.clone());

				//Hacemos un update normal del usuario
				updateData(localData.getUserDirtyData());
			}
			else
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Getting data from remote user.");
				}
				//Nos traemos los datos de este usuario
				server.getUserData(currentUser.id, localData.getCSVKeysToQuery(), localData.currentUser.PlayFab_dataVersion);	
			}
		}


		/**
		 * Recibiendo la data del usuario
		 **/
		protected override void OnDataReceived (string fullData)
		{
			base.OnDataReceived (fullData);

			localData.diffUser(JsonUtility.FromJson<KuberaUser>(fullData), true);

			if(_mustShowDebugInfo)
			{
				Debug.Log("Usuario sincronizado.");
			}

			//Necesita subirse?
			if(localData.currentUser.isDirty)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Subiendo datos sucios del usuario.");
				}
				updateData(localData.getUserDirtyData());
			}
		}

		/**
		 * Manda actualizar los datos del usuario
		 **/ 
		public void updateData(KuberaUser dirtyUser)
		{
			if(_mustShowDebugInfo)
			{
				Debug.Log("To update: \n"+userToPlayFabJSON(dirtyUser));
			}

			//Si no hay usuario remoto entonces no hay nada que actualizar
			if(existCurrentUser())
			{
				server.updateUserData(currentUser.id, userToPlayFabJSON(dirtyUser), dirtyUser);
			}
		}

		/**
		 * Una vez actualizados los datos hay que actualizar el estado local de los datos
		 **/ 
		protected override void OnDataUpdated (string updatedData)
		{
			base.OnDataUpdated (updatedData);

			KuberaUser updatedUser = JsonUtility.FromJson<KuberaUser>(updatedData);

			//Se actualizo algo en el server
			localData.diffUser(updatedUser, true);//Ignoramos la version

			if(_mustShowDebugInfo)
			{
				Debug.Log("Usuario sincronizado.");
			}

			//Necesita subirse?
			if(localData.currentUser.isDirty)
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log("Subiendo datos sucios del usuario.");
				}
				updateData(localData.getUserDirtyData());
			}
		}

		/**
		 * Usuario en el formato:
		 * {
		 * 	"id":string,
		 * 	"facebookId":string,
		 * 	"version":int,
		 * 	"PlayFab_dataVersion":int,
		 * 	"world_nn":WorldData (cualquier cantidad de mundos donde nn es igual a su id)
		 * }
		 **/ 
		public string userToPlayFabJSON(KuberaUser user)
		{
			StringBuilder builder = new StringBuilder("{");
			//builder.Append(quotted("id")+":"+quotted(user.id)+","+quotted("facebookId")+":"+quotted(user.facebookId)+","+quotted("version")+":"+user.version.ToString()+","+quotted("PlayFab_dataVersion")+":"+user.PlayFab_dataVersion.ToString());
			builder.Append(quotted("id")+":"+quotted(user.id)+","+quotted("facebookId")+":"+quotted(user.facebookId)+","+quotted("version")+":"+user.version.ToString());

			//agregamos los mundos
			foreach(WorldData world in user.worlds)
			{
				builder.Append(",");
				builder.Append(quotted("world_"+world.id)+":");
				builder.Append(JsonUtility.ToJson(world));
			}

			//Helper para el servidor
			builder.Append(","+quotted("world_count")+":"+user.worlds.Count);

			builder.Append("}");
			return builder.ToString();
		}

		public string quotted(string target){return "\""+target+"\"";}
	}
		
}