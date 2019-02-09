using Newtonsoft.Json;
using ROFLPlayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROFLPlayer.Utilities
{
    public class ExecsManager
    {

        public static string ExecsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "execs");

        public static string[] GetSavedExecs()
        {
            if (!Directory.Exists(ExecsFolder))
            {
                Directory.CreateDirectory(ExecsFolder);
                return new string[0];
            }

            return Directory.GetFiles(ExecsFolder).Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
        }

        public static LeagueExecutable GetExec(string name)
        {
            var filePath = Path.Combine(ExecsFolder, name + ".json");

            if(!File.Exists(filePath)) { return null; }

            return JsonConvert.DeserializeObject<LeagueExecutable>(File.ReadAllText(filePath));
        }

        public static bool DeleteExecFile(string name)
        {
            return false;
        }

        public static string SaveExecFile(LeagueExecutable leagueExecutable)
        {
            if (leagueExecutable == null) { return null; }

            var filePath = Path.Combine(ExecsFolder, leagueExecutable.Name + ".json");

            if (File.Exists(filePath))
            {
                return "!EXISTS";
            }

            if(!Directory.Exists(ExecsFolder))
            {
                Directory.CreateDirectory(ExecsFolder);
            }

            var serialized = JsonConvert.SerializeObject(leagueExecutable);

            try
            {
                File.WriteAllText(filePath, serialized);
            }
            catch (Exception ex)
            {
                return "!FAIL" + ex.ToString();
            }

            return filePath;
        }
    }
}
