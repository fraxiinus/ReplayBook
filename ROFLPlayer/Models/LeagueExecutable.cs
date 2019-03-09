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

        public bool AllowUpdates { get; set; }
        public bool IsDefault { get; set; }

        public DateTime ModifiedDate { get; set; }

        public LeagueExecutable() { }

        public LeagueExecutable(string _name, string _targetPath, string _startFolder, string _patchVer, bool _allowUpdates, bool _isDefault, DateTime _modifDate)
        {
            Name = _name;
            TargetPath = _targetPath;
            StartFolder = _startFolder;
            PatchVersion = _patchVer;
            AllowUpdates = _allowUpdates;
            IsDefault = _isDefault;
            ModifiedDate = _modifDate;
        }

        public override string ToString()
        {
            return $"{Name}\tUpdates:{AllowUpdates}\n{TargetPath}";
        }
    }
}
