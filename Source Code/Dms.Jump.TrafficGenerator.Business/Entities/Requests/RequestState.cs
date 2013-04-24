using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;

namespace Dms.Jump.TrafficGenerator.Business.Entities.Requests
{
    /// <summary>
    /// The RequestState class passes data across async calls. 
    /// </summary>
    public class RequestState
    {
        const int BufferSize = 1024;
        public StringBuilder RequestData;
        public byte[] BufferRead;
        public WebRequest Request;
        public Stream ResponseStream;
        // Create Decoder for appropriate enconding type.
        public Decoder StreamDecode = Encoding.UTF8.GetDecoder();

        public RequestState()
        {
            BufferRead = new byte[BufferSize];
            RequestData = new StringBuilder(String.Empty);
            Request = null;
            ResponseStream = null;
        }
    }
}