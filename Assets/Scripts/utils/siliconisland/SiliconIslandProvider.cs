using UnityEngine;
using System;
using System.Collections;

using Data.Remote;

using utils.gems.remote.request;
using utils.gems.remote.response;

namespace utils.gems.remote
{
	public class SiliconIslandProvider: ServerProvider 
	{

		private SPGetGemsRequest mainUpdateRequest;

		private string GP_API = "https://api-2445581383014.staging.apicast.io:443";

		public void test ()
		{
			//GMRequest request = queue.getComponentAttachedToGameObject<GMRequest>("GM_Request");
			GMRequestUsers request = queue.getComponentAttachedToGameObject<GMRequestUsers>("GM_Request");
			request.persistAfterFailed = true;
			request.showDebugInfo = _mustShowDebugInfo;
			request.initialize(GP_API);

			addRequest(request,true);
		}

		private void addDependantRequest(BaseRequest request, bool isPriority = false)
		{

			//El usuario se esta actualizando por primera vez?
			if(mainUpdateRequest != null && mainUpdateRequest.id != request.id)
			{
				//request dependiente de que se termine de actualizar el usuario
				mainUpdateRequest.dependantRequests.Add(request);
			}
			else
			{
				//request normal
				addRequest(request, isPriority);
			}
		}

		public override void stopAndRemoveCurrentRequests ()
		{
			base.stopAndRemoveCurrentRequests ();
			mainUpdateRequest = null;
		}
	}

}