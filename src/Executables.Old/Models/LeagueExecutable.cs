using Fraxiinus.ReplayBook.Configuration.Models;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Fraxiinus.ReplayBook.Executables.Old.Models;

public class LeagueExecutable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private string _name;
    [JsonPropertyName("name")]
    public string Name 
    { 
        get { return _name; }
        set
        {
            _name = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(Name)));
        } 
    }

    private string _targetPath;
    [JsonPropertyName("targetPath")]
    public string TargetPath
    {
        get { return _targetPath; }
        set
        {
            _targetPath = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(TargetPath)));
        }
    }

    private string _startFolder;
    [JsonPropertyName("startFolder")]
    public string StartFolder
    {
        get { return _startFolder; }
        set
        {
            _startFolder = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(StartFolder)));
        }
    }

    private string _patchNumber;
    [JsonPropertyName("patchNumber")]
    public string PatchNumber
    {
        get { return _patchNumber; }
        set
        {
            _patchNumber = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(PatchNumber)));
        }
    }

    private string _launchArgs;
    [JsonPropertyName("launchArgs")]
    public string LaunchArguments
    {
        get { return _launchArgs; }
        set
        {
            _launchArgs = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(LaunchArguments)));
        }
    }

    private LeagueLocale _locale;
    [JsonPropertyName("locale")]
    public LeagueLocale Locale
    {
        get { return _locale; }
        set
        {
            _locale = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(Locale)));
        }
    }

    private string _customLocale;
    [JsonPropertyName("customLocale")]
    public string CustomLocale 
    { 
        get { return _customLocale; }
        set
        {
            _customLocale = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(CustomLocale)));
        }
    }

    private DateTime _modifiedDate;
    [JsonPropertyName("modifiedDate")]
    public DateTime ModifiedDate
    {
        get { return _modifiedDate; }
        set
        {
            _modifiedDate = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(ModifiedDate)));
        }
    }
}
