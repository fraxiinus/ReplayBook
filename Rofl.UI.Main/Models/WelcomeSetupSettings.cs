using Rofl.Executables.Models;
using Rofl.Executables.Utilities;
using Rofl.Settings.Models;
using System.Collections.Generic;

namespace Rofl.UI.Main.Models
{
    public class WelcomeSetupSettings
    {
        public string RiotGamesPath { get; set; }

        public IList<LeagueExecutable> Executables { get; set; }

        public string ReplayPath { get; set; }

        public LeagueLocale DefaultRegionLocale { get; set; }

        public Language SetupLanguage { get; set; }
    }
}
