using System;
using System.Drawing;

namespace Rofl.Requests.Models
{
    public class ResponseBase
    {
        public string ResponsePath { get; set; }

        public DateTime ResponseDate { get; set; }

        public string RequestUrl { get; set; }

        public string DataVersion { get; set; }

        public bool IsFaulted { get; set; }
        
        public bool FromCache { get; set; }

        public Exception Exception { get; set; }

        public RequestBase Request { get; set; }
        
        public byte[] ResponseBytes { get; set; }
    }
}
