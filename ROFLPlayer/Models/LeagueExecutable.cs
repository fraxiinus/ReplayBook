using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROFLPlayer.Models
{
    public class LeagueExecutable
    {
        public string Name { get; set; }
        public string TargetPath { get; set; }
        public string StartFolder { get; set; }
        public string PatchVersion { get; set; }

        public bool EnableUpdates { get; set; }
        public bool IsDefault { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string ToString()
        {
            return $"{Name}\tUpdates:{EnableUpdates}\n{TargetPath}";
        }
    }
}
