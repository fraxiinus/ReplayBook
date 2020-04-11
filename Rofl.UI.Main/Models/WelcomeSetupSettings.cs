using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rofl.Executables.Utilities;

namespace Rofl.UI.Main.Models
{
    public class WelcomeSetupSettings
    {
        public string RiotGamesPath { get; set; }

        public string ReplayPath { get; set; }

        public LeagueLocale RegionLocale { get; set; }
    }
}
