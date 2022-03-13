using Fraxiinus.ReplayBook.Configuration.Models;
using Fraxiinus.ReplayBook.Executables.Old;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraxiinus.ReplayBook.UI.Main.Models
{
    public class SettingsWindowDataContext
    {
        public ObservableConfiguration Configuration { get; set; }

        public ExecutableManager Executables { get; set; }
    }
}
