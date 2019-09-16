using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rofl.UI.Main.Models
{
    public class PlayerPreviewModel
    {
        public string ChampionName { get; set; }

        public string PlayerName { get; set; }

        public bool IsKnownPlayer { get; set; }

        public string CombinedName
        {
            get
            {
                return $"{PlayerName} - {ChampionName}";
            }
        }
    }
}
