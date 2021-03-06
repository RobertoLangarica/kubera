﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Data;
using Kubera.Data.Sync;
using Kubera.Data.Remote.PFResponseData;

namespace Kubera.Data
{
	public class DataManagerKubera : LocalDataManager<MultipleUsers>
	{
		public KuberaSyncManger syncManager;
		public int initialLifes = 5;
		public int maximumLifesPossible = 7;
		protected Levels levelsList;

		protected override void Start ()
		{
			base.Start ();

			#if UNITY_EDITOR
			if(PersistentData.GetInstance() != null)
			{
				levelsList = PersistentData.GetInstance().levelsData;
			}
			#else
			levelsList = PersistentData.GetInstance().levelsData;
			#endif
		}

		public KuberaUser currentUser{get{return currentData.getUserById(currentUserId);}}

		protected override void fillDefaultData ()
		{
			base.fillDefaultData ();

			//Usuario anonimo
			KuberaUser anonymous = new KuberaUser(ANONYMOUS_USER);
			cleanToAnonymousData(anonymous);
			currentData.users.Add(anonymous);
		}

		public bool alreadyUseGems()
		{
			return currentUser.gemsUse;
		}

		public bool alreadyPurchaseGems()
		{
			return currentUser.gemsPurchase;
		}

		public bool alreadyUseGemsAfterPurchase()
		{
			return currentUser.gemsUseAfterPurchase;
		}

		public bool alreadyAskForLifes()
		{
			return currentUser.lifesAsked;
		}

		public void markGemsAsUsed()
		{
			KuberaUser currUser = currentUser;

			currUser.isDirty = currUser.updateGemsUse(true) || currUser.isDirty;

			if(currUser.isDirty)
			{
				saveLocalData(false);

				//Mandamos el dato al server
				KuberaUser user = new KuberaUser(currentUserId);
				user.gemsUse = true;

				syncManager.updateData(user);	
			}
		}

		public void markGemsAsPurchased()
		{
			KuberaUser currUser = currentUser;

			currUser.isDirty = currUser.updateGemsPurchase(true) || currUser.isDirty;

			if(currUser.isDirty)
			{
				saveLocalData(false);

				//Mandamos el dato al server
				KuberaUser user = new KuberaUser(currentUserId);
				user.gemsPurchase = true;

				syncManager.updateData(user);	
			}
		}

		public void markGemsAsUsedAfterPurchased()
		{
			KuberaUser currUser = currentUser;

			currUser.isDirty = currUser.updateGemsAfterPurchase(true) || currUser.isDirty;

			if(currUser.isDirty)
			{
				saveLocalData(false);

				//Mandamos el dato al server
				KuberaUser user = new KuberaUser(currentUserId);
				user.gemsUseAfterPurchase = true;

				syncManager.updateData(user);	
			}
		}

		public void markLifesAsAsked()
		{
			KuberaUser currUser = currentUser;

			currUser.isDirty = currUser.updateLifesAsked(true) || currUser.isDirty;

			if(currUser.isDirty)
			{
				saveLocalData(false);

				//Mandamos el dato al server
				KuberaUser user = new KuberaUser(currentUserId);
				user.lifesAsked = true;

				syncManager.updateData(user);	
			}
		}

		public void incrementLevelAttemp(string levelName)
		{
			KuberaUser currUser = currentUser;
			LevelData level = currUser.getLevelById(levelName);


			if(level != null)
			{
				level.attempts++;
				level.isDirty = true;
			}
			else
			{
				level = new LevelData(levelName);
				level.points	= 0;
				level.stars		= 0;
				level.passed	= false;
				level.world		= levelsList.getLevelByName(levelName).world;
				level.attempts	= 1;
				level.isDirty	= true;

				currUser.addLevel(level);
			}

			KuberaAnalytics.GetInstance ().registerLevelAttempts (levelName,level.attempts);
				
			currUser.isDirty = true;


			if(currUser.isDirty)
			{
				saveLocalData(false);

				//Mandamos un usuario solo con este nivel
				KuberaUser user = new KuberaUser(currentUserId);
				user.addLevel(level);

				syncManager.updateData(user);	
			}
		}

		public void savePassedLevel(string levelName, int stars, int points)
		{
			KuberaUser currUser = currentUser;
			LevelData level = currUser.getLevelById(levelName);


			if(level != null)
			{
				KuberaAnalytics.GetInstance ().registerFirstWinAttempts (levelName,level.attempts);
				level.updateOnlyIncrementalValues(stars, points);
				level.updatePassed(true);
				level.attempts++;
				level.isDirty = true;
			}
			else
			{
				level = new LevelData(levelName);
				level.points	= points;
				level.stars		= stars;
				level.passed	= true;
				level.world		= levelsList.getLevelByName(levelName).world;
				level.attempts	= 1;
				level.isDirty	= true;

				currUser.addLevel(level);

				KuberaAnalytics.GetInstance ().registerFirstWinStars (levelName,stars);
				KuberaAnalytics.GetInstance ().registerFirstWinAttempts (levelName,level.attempts);
			}

			KuberaAnalytics.GetInstance ().registerLevelAttempts (levelName,level.attempts);

			//El maximo avance
			currUser.isDirty = currUser.upgradeMaxLevelReached(int.Parse(level._id)) || currUser.isDirty;
				
			//Es sucio porque ya estaba sucio o por un cambio aqui
			currUser.isDirty = currUser.isDirty || level.isDirty;

			if(currUser.isDirty)
			{
				saveLocalData(false);

				//Mandamos un usuario solo con este nivel
				KuberaUser user = new KuberaUser(currentUserId);
				user.addLevel(level);

				syncManager.updateData(user);	
			}
		}

		public bool isLevelPassed(string levelName)
		{
			LevelData level = currentUser.getLevelById(levelName);

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
				if(world[0].world == 1)
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
			//else if(index > 1)
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
			LevelData level =  currentUser.getLevelById(levelName);

			if(level != null)
			{
				return level.locked;
			}

			return true;
		}

		public void unlockLevel(string levelName)
		{
			LevelData level =  currentUser.getLevelById(levelName);

			if(level != null)
			{
				level.isDirty = level.updateLocked(false) || level.isDirty;
			}
			else
			{
				level = new LevelData(levelName);
				level.locked	= false;
				level.world		= levelsList.getLevelByName(levelName).world;
				level.isDirty	= true;

				currentUser.addLevel(level);
			}

			//Cuidamos de no sobreescribir algun valor previo
			currentUser.isDirty = currentUser.isDirty || level.isDirty;

			if(currentUser.isDirty)
			{
				saveLocalData(false);

				//mandamos un usuario solo con este nivel
				KuberaUser user = new KuberaUser(currentUserId);
				user.addLevel(level);

				syncManager.updateData(user);
			}
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
			LevelData level = currentUser.getLevelById(levelName);

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
			LevelData level = currentUser.getLevelById(levelName);

			if(level == null)
			{
				return 0;
			}

			return level.points;
		}

		public int getLevelAttempts(string levelName)
		{
			LevelData level = currentUser.getLevelById(levelName);

			if(level == null)
			{
				return 0;
			}

			return level.attempts;
		}

		public void giveUserLifes(int amount = 1)
		{
			int totalLifes = currentUser.playerLifes + amount;

			//Los usuarios anonimos solo tienen derechos a las vidas iniciales
			if (totalLifes > initialLifes && currentUserId == ANONYMOUS_USER) 
			{
				currentUser.playerLifes = initialLifes;
			} 
			else if (totalLifes < 0) 
			{
				//nunca menos de 0 vidas
				currentUser.playerLifes = 0;
			}
			else 
			{
				//que no se acumulen cuando un usuario tiene mas vidas por estar conectado a facebook
				if(totalLifes > maximumLifesPossible)
				{
					totalLifes = maximumLifesPossible;
				}

				currentUser.playerLifes = totalLifes;
			}

			saveLocalData(false);
		}

		public void temporalUserChangeWithFacebookId(string facebookId)
		{
			string newId;
			KuberaUser user;
			KuberaUser anon;

			if(currentUserId == ANONYMOUS_USER)
			{
				//El nuevo usuario existe?
				if(currentData.getUserByFacebookId(facebookId) == null)
				{
					//NO existe y creamos uno nuevo
					user = currentData.getUserById(ANONYMOUS_USER);


					anon = new KuberaUser(ANONYMOUS_USER);
					newId = facebookId;
					//Valores iniciales
					cleanToAnonymousData(anon);

					//Este usuario anonimo pertenece a otro id de facebook?
					if(user.facebookId != "")
					{
						//Un nuevo usuario para este que llego
						anon._id = facebookId;	
						anon.facebookId = facebookId;

					}
					else
					{
						//este anonimo se convierte en el de facebook que llego
						user._id = facebookId;
						user.facebookId = facebookId;
					}

					currentData.users.Add(anon);
				}
				else
				{
					//Ya existe
					//Diff de los datos sin verificar version
					user = currentData.getUserByFacebookId(facebookId);
					anon = currentData.getUserById(ANONYMOUS_USER);

					newId = user._id;

					//Solo se hace un diff si el anonimo no pertenecea ningun usuario de facebook
					if(user.facebookId == "")
					{
						//prevalece la version del usuario que no es anonimo
						currentUser.remoteDataVersion = user.remoteDataVersion;
						user.compareAndUpdate(currentUser, true);


						//Limpiamos al usuario anonimo para que tenga valores iniciales
						cleanToAnonymousData(currentUser);
					}
				}
			}
			else
			{
				//El nuevo usuario existe?
				if(currentData.getUserByFacebookId(facebookId) == null)
				{
					//Se crea un nuevo usuario
					user = new KuberaUser(facebookId);
					user.facebookId = facebookId;
					user.playerLifes = initialLifes;

					newId = facebookId;

					currentData.users.Add(user);

				}
				else
				{
					//Hay un cambio de usuario sin consecuencias
					newId = currentData.getUserByFacebookId(facebookId)._id;
				}
			}

			currentUserId = newId;
			saveLocalData(false);
		}
			
		public override void changeCurrentuser (string newUserId)
		{
			if(currentUserId == newUserId)
			{
				//No hay cambios que hacer
				return;	
			}

			//NOTA: Aqui por el flujo de login teoricamente nunca se llega con usuario anonimo pero es un reality check

			//Si es anonimo hay que ver si los avances se guardan
			if(currentUserId == ANONYMOUS_USER)
			{
				//El nuevo usuario existe?
				if(currentData.getUserById(newUserId) == null)
				{
					//Este usuario toma los datos anonimos
					currentData.getUserById(ANONYMOUS_USER)._id = newUserId;

					//Agregamos un nuevo usuario anonimo
					KuberaUser anon = new KuberaUser(ANONYMOUS_USER);
					cleanToAnonymousData(anon);
					currentData.users.Add(anon);
				}
				else
				{
					//Diff de los datos sin verificar version
					currentData.getUserById(newUserId).compareAndUpdate(currentUser, true);

					//Limpiamos al usuario anonimo para que tenga valores iniciales
					cleanToAnonymousData(currentUser);
				}
			}
			else
			{
				//El nuevo usuario existe?
				if(currentData.getUserById(newUserId) == null)
				{
					KuberaUser newUser = new KuberaUser(newUserId);
					newUser.playerLifes = initialLifes;
					currentData.users.Add(newUser);
				}
				else
				{
					//Hay un cambio de usuario sin consecuencias
				}
			}

			base.changeCurrentuser (newUserId);

			//currentData.isDirty = currentUser.isDirty;
			saveLocalData(false);
		}
			
		private void cleanToAnonymousData(KuberaUser user)
		{
			user.clear();
			user._id = ANONYMOUS_USER;
			user.playerLifes = initialLifes;
		}

		public override void setUserAsAnonymous ()
		{
			//Reality check
			if(currentUserId == ANONYMOUS_USER)
			{
				return;	
			}

			/*El current user se hace anonimo y mantiene su facebook ID,
			el anonimo previo se elimina con todos sus datos*/
			KuberaUser anon = currentData.getUserById(ANONYMOUS_USER);
			if(anon != null)
			{
				currentData.users.Remove(anon);
			}

			currentUser._id = ANONYMOUS_USER;

			base.setUserAsAnonymous ();
		}

		public void diffUser(KuberaUser remoteUser, bool ignoreVersion = false)
		{
			if(currentUserId != remoteUser._id)
			{
				Debug.Log("Se recibieron datos de otro usuario: "+currentUserId+","+ remoteUser._id);	
				return;
			}

			currentUser.compareAndUpdate(remoteUser, ignoreVersion);
			saveLocalData(false);
		}

		/**
		 * Los datos de este usuario que necesiten subirse
		 **/ 
		public KuberaUser getUserDirtyData()
		{
			KuberaUser user = currentUser;
			KuberaUser result = new KuberaUser(user._id);

			result.facebookId = user.facebookId;
			result.levels = user.getDirtyLevelsCopy();
			result.maxLevelReached = user.maxLevelReached;
			result.gemsUse = user.gemsUse;
			result.gemsPurchase = user.gemsPurchase;
			result.gemsUseAfterPurchase = user.gemsUseAfterPurchase;
			result.lifesAsked = user.lifesAsked;
			result.remoteLifesGranted = user.remoteLifesGranted;

			//Se envian como no sucios
			result.markAllLevelsAsNoDirty();

			return result;
		}
	}
}
