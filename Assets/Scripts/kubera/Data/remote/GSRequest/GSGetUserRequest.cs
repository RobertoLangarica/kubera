using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using BestHTTP;

using Data.Remote;
using Kubera.Data.Remote.GSResponseData;

namespace Kubera.Data.Remote
{
	public class GSGetUserRequest :  RemoteRequest<GSGetUserResponse>
	{
		public string playerId;
		public int aboveVersion;

		public override string getCompletePath (string path)
		{
			return path+"/LogEventRequest";
		}

		public override void initialize (string path)
		{
			request = new HTTPRequest(new System.Uri(getCompletePath(path)), HTTPMethods.Post);

			request.ConnectTimeout = TimeSpan.FromSeconds(timeBeforeTimeout);
			request.Timeout = TimeSpan.FromSeconds(timeBeforeTimeout*3);
			request.AddField(quotedString("@class"), quotedString(".LogEventRequest"));
			request.AddField(quotedString("eventKey"), quotedString("GET_DATA"));
			request.AddField(quotedString("above_version"),aboveVersion.ToString());
			request.AddField(quotedString("requestId"),quotedString(this.id));
			request.AddField(quotedString("playerId"),quotedString(playerId));
			request._showDebugInfo = showDebugInfo;
			request.FormUsage = BestHTTP.Forms.HTTPFormUsage.App_JSON;
		}

		protected override void formatData(string dataAsText)
		{
			Dictionary<string,object> preData = MiniJSON.Json.Deserialize(dataAsText) as Dictionary<string,object>;

			data = new GSGetUserResponse();
			data.@class = preData["@class"].ToString();

			if(preData.ContainsKey("error"))
			{
				//data con error
				data.error = preData["error"].ToString();
			}
			else
			{
				//data sin error
				Dictionary<string,object> scriptData = (preData["scriptData"] as Dictionary<string,object>)["userData"] as Dictionary<string,object>;

				if(scriptData.ContainsKey("version"))
				{
					data.version = int.Parse(scriptData["version"].ToString());
				}
				else
				{
					data.version = -1;
				}

				if(scriptData.ContainsKey("maxLevelReached"))
				{
					data.version = int.Parse(scriptData["maxLevelReached"].ToString());
				}
				else
				{
					data.maxLevelReached = -1;
				}

				if(scriptData.ContainsKey("gemsUse"))
				{
					data.gemsUse = int.Parse(scriptData ["gemsUse"].ToString()) == 1 ? true:false;
				}
				else
				{
					data.gemsUse = false;
				}

				if(scriptData.ContainsKey("gemsPurchase"))
				{
					data.gemsPurchase = int.Parse(scriptData ["gemsPurchase"].ToString()) == 1 ? true:false;
				}
				else
				{
					data.gemsPurchase = false;
				}

				if(scriptData.ContainsKey("gemsUseAfterPurchase"))
				{
					data.gemsUseAfterPurchase = int.Parse(scriptData ["gemsUseAfterPurchase"].ToString()) == 1 ? true:false;
				}
				else
				{
					data.gemsUseAfterPurchase = false;
				}

				if(scriptData.ContainsKey("lifesAsked"))
				{
					data.lifesAsked = int.Parse(scriptData ["lifesAsked"].ToString()) == 1 ? true:false;
				}
				else
				{
					data.lifesAsked = false;
				}

				if(scriptData.ContainsKey("remoteLifesGranted"))
				{
					data.remoteLifesGranted = int.Parse(scriptData ["remoteLifesGranted"].ToString()) == 1 ? true:false;
				}
				else
				{
					data.remoteLifesGranted = false;
				}
					
				if(scriptData.ContainsKey("levels"))
				{
					Dictionary<string,object> levelsData = 	scriptData["levels"] as Dictionary<string,object>;
					List<LevelData> levels = new List<LevelData>();

					foreach(KeyValuePair<string, object> item in levelsData)
					{
						LevelData level = new LevelData(item.Key);
						Dictionary<string,object> value = item.Value as Dictionary<string,object>;
						level.points	= int.Parse(value["points"].ToString());
						level.stars		= int.Parse(value["stars"].ToString());
						level.world 	= int.Parse(value["world"].ToString());
						level.passed	= int.Parse(value ["passed"].ToString()) == 1 ? true:false;
						level.locked	= int.Parse(value ["locked"].ToString()) == 1 ? true:false;

						//una version de niveles no tenia intentos y evitamos el error
						if(value.ContainsKey("attempts"))
						{
							level.attempts	= int.Parse(value ["attempts"].ToString());	
						}
						else
						{
							level.attempts = 0;
						}
						levels.Add(level); 	
					}

					data.levels = levels;
				}
			}

		}

		public override bool hasError ()
		{
			return (data == null || data.error != null);
		}
	}

}