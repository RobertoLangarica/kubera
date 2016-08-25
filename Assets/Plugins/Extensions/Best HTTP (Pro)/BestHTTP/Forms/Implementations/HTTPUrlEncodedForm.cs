using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BestHTTP.Forms
{
    /// <summary>
    /// A HTTP Form implementation to send textual values.
    /// </summary>
    public sealed class HTTPUrlEncodedForm : HTTPFormBase
    {
        private byte[] CachedData;

        public override void PrepareRequest(HTTPRequest request)
        {
            request.SetHeader("Content-Type", "application/x-www-form-urlencoded");
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

            // Create a "field1=value1&field2=value2" formatted string
            for (int i = 0; i < Fields.Count; ++i)
            {
                var field = Fields[i];

                if (i > 0)
                    sb.Append("&");

                sb.Append(Uri.EscapeDataString(field.Name));
                sb.Append("=");

                if (!string.IsNullOrEmpty(field.Text) || field.Binary == null)
				{
					sb = EscapeDataAndAppend(sb,field.Text);
                    //sb.Append(Uri.EscapeDataString(field.Text));
				}
                else
				{
                    // If forced to this form type with binary data, we will create a string from the binary data first and encode this string.
					sb = EscapeDataAndAppend(sb,Encoding.UTF8.GetString(field.Binary, 0, field.Binary.Length));
                   // sb.Append(Uri.EscapeDataString(Encoding.UTF8.GetString(field.Binary, 0, field.Binary.Length)));
				}
            }

            IsChanged = false;
            return CachedData = Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}