using UnityEngine;
using System;
using System.Collections;
using System.Text;

namespace BestHTTP.Forms
{
	public class HTTPAppJSONForm : HTTPFormBase
	{
		private byte[] CachedData;

		public override void PrepareRequest(HTTPRequest request)
		{
			//Debug.Log("Preparing HEADER");
			request.SetHeader("Content-Type", "application/json");
		}

		private StringBuilder EscapeDataAndAppend(StringBuilder dest,string toAppend)
		{
			int limit = 2000;
			int loops = toAppend.Length/limit;

			for(int i = 0; i <= loops; i++)
			{
				if(i < loops)
				{
					dest.Append(Uri.EscapeDataString(toAppend.Substring(limit*i,limit)));
				}
				else
				{
					dest.Append(Uri.EscapeDataString(toAppend.Substring(limit*i)));
				}
			}
			return dest;
		}

		public override byte[] GetData()
		{
			if (CachedData != null && !IsChanged)
				return CachedData;

			StringBuilder sb = new StringBuilder();

			//JSON
			sb.Append("{");
			for (int i = 0; i < Fields.Count; ++i)
			{
				var field = Fields[i];

				if (i > 0)
					sb.Append(",");

				//sb = EscapeDataAndAppend(sb,field.Name);
				sb.Append(field.Name);
				sb.Append(":");

				if (!string.IsNullOrEmpty(field.Text) || field.Binary == null)
				{
					sb.Append(field.Text);
					//sb.Append(Uri.EscapeDataString(field.Text));
				}
				else
				{
					//sb = EscapeDataAndAppend(sb,Encoding.UTF8.GetString(field.Binary, 0, field.Binary.Length));
					sb.Append(Uri.EscapeDataString(Encoding.UTF8.GetString(field.Binary, 0, field.Binary.Length)));
				}
			}
			sb.Append("}");
			IsChanged = false;
			Debug.Log(sb.ToString());
			return CachedData = Encoding.UTF8.GetBytes(sb.ToString());
		}
	}	
}

