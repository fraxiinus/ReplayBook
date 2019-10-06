using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Rofl.Requests.Models
{
    public class ResponseBase
    {
        public string ResponsePath { get; set; }

        public DateTime ResponseDate { get; set; }

        public string RequestUrl { get; set; }

        public string DataVersion { get; set; }

        public bool IsFaulted { get; set; }

        public Exception Exception { get; set; }

        public RequestBase Request { get; set; }
    }
}
