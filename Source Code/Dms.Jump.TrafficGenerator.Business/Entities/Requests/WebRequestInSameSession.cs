using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace Dms.Jump.TrafficGenerator.Business.Entities.Requests
{
    public class WebRequestInSameSession
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private const int BUFFER_SIZE = 0x400;

        public static void PerformRequest(Uri uri)
        {
            PerformRequest(uri, null); 
        }

        // Methods
        public static void PerformRequest(Uri uri, CookieContainer cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = cookies;

            request.Timeout = 200;
            RequestState rs = new RequestState();
            rs.Request = request;
            IAsyncResult r = request.BeginGetResponse(new AsyncCallback(WebRequestInSameSession.RespCallback), rs);
            allDone.WaitOne();
        }

        private static void ReadCallBack(IAsyncResult asyncResult)
        {
            RequestState rs = (RequestState)asyncResult.AsyncState;
            Stream responseStream = rs.ResponseStream;
            int read = responseStream.EndRead(asyncResult);
            if (read > 0)
            {
                char[] charBuffer = new char[0x400];
                int len = rs.StreamDecode.GetChars(rs.BufferRead, 0, read, charBuffer, 0);
                string str = new string(charBuffer, 0, len);
                rs.RequestData.Append(Encoding.ASCII.GetString(rs.BufferRead, 0, read));
                IAsyncResult ar = responseStream.BeginRead(rs.BufferRead, 0, 0x400, new AsyncCallback(WebRequestInSameSession.ReadCallBack), rs);
            }
            else
            {
                if (rs.RequestData.Length > 0)
                {
                    string strContent = rs.RequestData.ToString();
                }
                responseStream.Close();
                allDone.Set();
            }
        }

        private static void RespCallback(IAsyncResult ar)
        {
            RequestState rs = (RequestState)ar.AsyncState;
            Stream ResponseStream = rs.Request.EndGetResponse(ar).GetResponseStream();
            rs.ResponseStream = ResponseStream;
            IAsyncResult iarRead = ResponseStream.BeginRead(rs.BufferRead, 0, 0x400, new AsyncCallback(WebRequestInSameSession.ReadCallBack), rs);
        }
    }
}