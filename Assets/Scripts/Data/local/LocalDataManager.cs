using UnityEngine;
using System;
using System.IO;
using System.Collections;

namespace Data
{
	public class LocalDataManager<T> : Manager<LocalDataManager<T>> where T:BasicData
	{
		public string ANONYMOUS_USER = "anon_user";
		public string currentUserId = ANONYMOUS_USER;

		public string mainDirectoryName = "Data";

		protected T currentData;

		protected virtual void Start()
		{
			createLocalFileStructure();

			//Nueva data
			if (!existLocalData()) 
			{
				saveLocalData (true);
			}
			else
			{
				readLocalData();
			}

		}

		private void createLocalFileStructure()
		{
			if(!Directory.Exists(getMainFolderPath()))
			{
				Directory.CreateDirectory(getMainFolderPath());
			}
		}

		public string getMainFolderPath()
		{
			return Application.persistentDataPath + "/" + mainDirectoryName;
		}

		public string getLocalDataPath()
		{
			return getMainFolderPath() + "/data.json";
		}

		public bool existLocalData()
		{
			return File.Exists (getLocalDataPath ());
		}

		public void saveLocalData(bool rewriteEmpty = false)
		{
			if (rewriteEmpty) 
			{
				currentData = Activator.CreateInstance<T>();
				fillDefaultData();
			}

			FileStream stream = new FileStream(getLocalDataPath(),FileMode.Create,FileAccess.Write,FileShare.None);
			StreamWriter streamWriter = new StreamWriter(stream);
			string jsonData = JsonUtility.ToJson(currentData,true);

			streamWriter.Write(jsonData);
			streamWriter.Close();
			streamWriter.Dispose();
			stream.Close();
			stream.Dispose();
		}

		protected virtual void fillDefaultData(){}

		public void readLocalData()
		{
			string json = File.ReadAllText(getLocalDataPath());
			currentData = JsonUtility.FromJson<T>(json);
		}

		protected virtual void syncDataWithServer()
		{
			/*HTTPRequest dataRequest = ServerProvider.GetInstance().getDataRequest();
			dataRequest.Callback = getDataCallback;
			dataRequest.Send();*/
		}

		/*private void getDataCallback(HTTPRequest request, HTTPResponse response)
		{
			if(ServerProvider.GetInstance ().isValidRequest (request) && response != null )
			{
				DataRoot parsedResponse = JsonUtility.FromJson<DataRoot> (response.DataAsText);
				diffData(parsedResponse);
			}

		}*/

		private void diffData(T fromServer)
		{
			if (currentData.compareAndUpdate(fromServer)) 
			{
				if(_mustShowDebugInfo)
				{
					Debug.Log ("<color=#f0f0ffff>Actualizando version local</color>");	
				}
				saveLocalData();

				//TODO: rutina para subir la informacion sucia al server
			}
			else if(_mustShowDebugInfo)
			{
				Debug.Log ("<color=#f0f0ffff>Version actualizada!!</color>");	
			}
		}

		public T getCurrentData()
		{
			return currentData;
		}

		public virtual void changeCurrentuser(string newUserId)
		{
			currentUserId = newUserId;
		}
	}
}