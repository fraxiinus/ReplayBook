using Etirps.RiZhi;
using System.Text.Json;
using Fraxiinus.ReplayBook.Executables.Old.Models;
using Fraxiinus.ReplayBook.Executables.Old.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.Executables.Old;

public class ExecutableManager
{

    private readonly RiZhi _log;

    private readonly string _exeInfoFilePath;

    private readonly JsonSerializerOptions serializerOptions = new()
    {
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public ExecutableSettings Settings { get; private set; }

    public ExecutableManager(RiZhi log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));

        // Get the exeInfoFile or create new one
        _exeInfoFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "executablesettings.json");
        if (!File.Exists(_exeInfoFilePath))
        {
            // Exe file is missing, set up defaults
            _log.Information($"Executable file does not exist, creating");
            Settings = new ExecutableSettings();
        }
        else
        {
            // Exe file found, load it
            try
            {
                var jsonText = File.ReadAllText(_exeInfoFilePath);
                Settings = JsonSerializer.Deserialize<ExecutableSettings>(jsonText, serializerOptions);
            }
            catch (Exception parseEx)
            {
                // Failed loading, create new one instead
                _log.Error($"Error reading executable info file, creating new one. {parseEx}");

                Settings = new ExecutableSettings();
            }
        }
    }

    ~ExecutableManager()
    {
        Save();
    }

    public async Task VerifyRegisteredExecutables()
    {
        foreach (LeagueExecutable executable in Settings.Executables)
        {
            if (File.Exists(executable.TargetPath))
            {
                await Task.Run(() => UpdateExecutableVersion(executable)).ConfigureAwait(true);
            }
            else
            {
                _log.Warning($"Target for executable '{executable.Name}' does not exist");
            }
        }
    }

    public (int, string[]) SearchAllFoldersForExecutablesAndAddThemAll()
    {
        int counter = 0;
        var skippedDirs = new List<string>();

        // loop through all source folders
        foreach (var path in Settings.SourceFolders)
        {
            // Look for executables
            IList<LeagueExecutable> foundExes;
            try
            {
                foundExes = SearchFolderForExecutables(path);
            }
            catch (DirectoryNotFoundException)
            {
                // If a directory does not exist, save it to be returned to sender
                skippedDirs.Add(path);
                continue;
            }

            foreach (var exe in foundExes)
            {
                // Check if target executable already exists
                if (GetExecutableByTarget(exe.TargetPath) == null)
                {
                    AddExecutable(exe);
                    counter++;
                }
            }
        }
        return (counter, skippedDirs.ToArray());
    }

    public string GetInstallationFolderFromRunningProcess()
    {
        if (!OperatingSystem.IsWindows())
        {
            _log.Error("ReplayBook is only intended to be run on Windows Operating Systems");
            return null;
        }

        IEnumerable<Process> leagueProcesses = Process.GetProcesses().Where(p => p.ProcessName.Contains("League"));
        foreach (Process process in leagueProcesses)
        {
            try
            {
                var stringBuilder = new StringBuilder(process.MainModule.FileName);

                stringBuilder.Append(' ');
                using (var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
                {
                    foreach (var searchResult in searcher.Get())
                    {
                        stringBuilder.Append(searchResult["CommandLine"]);
                        stringBuilder.Append(' ');
                    }
                }

                string commandLine = stringBuilder.ToString();
                int installDirIndex = commandLine.IndexOf("--install-directory");
                if (installDirIndex == -1)
                    continue;

                // Index started at "--league-directory=", but we now go to the start of the directory in the string
                installDirIndex = commandLine.IndexOf("=", installDirIndex) + 1;

                // Take everything until the " behind the directory
                return commandLine[installDirIndex..commandLine.IndexOf("\"", installDirIndex)];
            }
            catch (Win32Exception ex) when ((uint)ex.ErrorCode == 0x80004005)
            {
                // Intentionally empty.
            }
        }

        return null;
    }

    public IList<LeagueExecutable> SearchFolderForExecutables(string startPath)
    {
        var foundExecutables = new List<LeagueExecutable>();

        if (!Directory.Exists(startPath))
        {
            _log.Warning($"Input path {startPath} does not exist");
            throw new DirectoryNotFoundException($"Input path {startPath} does not exist");
        }

        try
        {
            // Look for any and all league of legends executables
            var exeFiles = Directory.EnumerateFiles(startPath, "League of Legends.exe", SearchOption.AllDirectories);

            foreach (string exePath in exeFiles)
            {
                LeagueExecutable newExe = null;

                try
                {
                    newExe = ExeTools.CreateNewLeagueExecutable(exePath);
                }
                catch (Exception ex)
                {
                    _log.Error($"{ex.GetType()} trying to create executable for path = \"{exePath}\"");
                    _log.Error(ex.ToString());
                    continue;
                }

                try
                {
                    newExe.Locale = ExeTools.DetectExecutableLocale(exePath);
                }
                catch (Exception ex)
                {
                    _log.Error($"{ex.GetType()} trying to find locale for path = \"{exePath}\"");
                    _log.Error(ex.ToString());
                    newExe.Locale = LeagueLocale.EnglishUS;
                    // do not stop operation
                }

                // Do we already have an exe with the same target?
                if (!foundExecutables.Exists(x => x.TargetPath.Equals(newExe.TargetPath, StringComparison.OrdinalIgnoreCase)))
                {
                    foundExecutables.Add(newExe);
                }
            }
        }
        catch (Exception e)
        {
            _log.Error(e.ToString());
            throw;
        }

        return foundExecutables;
    }

    public IEnumerable<LeagueExecutable> GetExecutables()
    {
        return Settings.Executables;
    }

    /// <summary>
    /// Returns <see cref="LeagueExecutable"/> with matching <paramref name="executableName"/>.
    /// If no matching item is found, returns null.
    /// If <paramref name="executableName"/> is null or empty then returns null.
    /// </summary>
    /// <param name="executableName"></param>
    /// <returns></returns>
    public LeagueExecutable GetExecutable(string executableName)
    {
        if (string.IsNullOrEmpty(executableName))
        {
            return null;
        }

        return Settings.Executables
            .Where(x => x.Name.Equals(executableName, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();
    }

    public LeagueExecutable GetExecutableByTarget(string target)
    {
        if (string.IsNullOrEmpty(target)) { return null; }

        return Settings.Executables
            .Where(x => x.TargetPath.Equals(target, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();
    }

    public IReadOnlyCollection<LeagueExecutable> GetExecutablesByPatch(string patchNumber)
    {
        var matching = Settings.Executables
            .Where(x => x.PatchNumber.VersionSubstring()
                .Equals(patchNumber.VersionSubstring(), StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return matching;
    }

    public bool DoesVersionExist(string patchNumber)
    {
        return GetExecutablesByPatch(patchNumber).Any();
    }

    public void AddExecutable(LeagueExecutable newExecutable)
    {
        // Validate file
        if (newExecutable == null)
        {
            _log.Warning($"Given Executable is null");
            throw new ArgumentNullException(nameof(newExecutable));
        }

        // Add character to the end of the name if already exists
        string name = newExecutable.Name;
        while (!CheckExecutableName(name))
        {
            name += "+";
        }
        newExecutable.Name = name;

        // Will throw exception if executable is invalid
        newExecutable.Validate();

        // Get the main UI thead to update the collection
        Application.Current.Dispatcher.Invoke((Action)delegate
        {
            Settings.Executables.Add(newExecutable);
        });
    }

    /// <summary>
    /// Deletes the item with name <paramref name="name"/>. Throws <see cref="ArgumentException"/> if name does not exist.
    /// If default is deleted, default is set to null.
    /// </summary>
    /// <param name="name"></param>
    public void DeleteExecutable(string name)
    {
        LeagueExecutable target = GetExecutable(name);
        if (target == null)
        {
            _log.Warning($"Executable with name {name} does not exist");
            throw new ArgumentException($"Executable with name {name} does not exist");
        }

        // Delete the executable
        _log.Information($"Deleting executable {target.Name}");

        Settings.Executables.Remove(target);
    }

    /// <summary>
    /// Write list of executables and default to file
    /// </summary>
    public void Save()
    {
        string outputFile = JsonSerializer.Serialize(Settings, serializerOptions);

        File.WriteAllText(_exeInfoFilePath, outputFile);
    }

    public bool CheckExecutableName(string name)
    {
        if (GetExecutable(name) != null) { return false; }
        return true;
    }

    public void UpdateExecutableVersion(LeagueExecutable executable)
    {
        if (executable == null) { throw new ArgumentNullException(nameof(executable)); }

        var currentVersion = ExeTools.GetLeagueVersion(executable.TargetPath);

        _log.Information($"Checking executable \'{executable.Name}\' for changes...");
        if (!executable.PatchNumber.Equals(currentVersion, StringComparison.OrdinalIgnoreCase))
        {
            _log.Information($"Updating executable {executable.Name} from {executable.PatchNumber} -> {currentVersion}");
            executable.PatchNumber = currentVersion;
        }
    }
}
