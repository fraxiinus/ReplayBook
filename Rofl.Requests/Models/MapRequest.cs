using System;
using System.Collections.Generic;
using System.Text;

namespace Rofl.Requests.Models
{
    public class MapRequest : RequestBase
    {
        public string MapName { get; set; }

        public string MapID { get; set; }
    }
}
