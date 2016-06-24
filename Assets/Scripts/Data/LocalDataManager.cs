using UnityEngine;
using System;
using System.IO;
using System.Collections;

namespace Data
{
	public class LocalDataManager<T> : Manager<LocalDataManager<T>> where T:BasicData
	{
		public string mainDirectoryName = "Data";

		public bool isSyncronized;

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

			isSyncronized = false;

			syncDataWithServer();
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

		//HACK: delete
		public void deleteData()
		{
			File.Delete (getLocalDataPath());

			saveLocalData (true);

			readLocalData();
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

		public void readLocalData()
		{
			string json = File.ReadAllText(getLocalDataPath());
			currentData = JsonUtility.FromJson<T>(json);
		}

		private void syncDataWithServer()
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

			afterServerSync();
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

		private void afterServerSync()
		{
			isSyncronized = true;
		}

		public T getCurrentData()
		{
			return currentData;
		}
	}
}