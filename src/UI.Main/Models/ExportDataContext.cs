namespace Fraxiinus.ReplayBook.UI.Main.Models;

using Etirps.RiZhi;
using Fraxiinus.ReplayBook.Files.Models;
using ModernWpf.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Fraxiinus.ReplayBook.UI.Main.Models.View;
using Fraxiinus.ReplayBook.StaticData;

/// <summary>
/// Set as the DataContext for <see cref="ExportDataWindow"/>
/// </summary>
public class ExportDataContext : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    #region Properties

    public RiZhi Log { get; set; }

    public StaticDataManager StaticDataManager { get; set; }

    public string LastPreset { get; set; }

    private Frame _contentFrame;
    /// <summary>
    /// Navigate to different exporter pages using this property
    /// </summary>
    public Frame ContentFrame
    {
        get => _contentFrame;
        set
        {
            _contentFrame = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(ContentFrame)));
        }
    }

    private string _windowTitleText;
    /// <summary>
    /// Change the exporter window title
    /// </summary>
    public string WindowTitleText
    {
        get => _windowTitleText;
        set
        {
            _windowTitleText = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(WindowTitleText)));
        }
    }

    private bool _hideHeader;
    /// <summary>
    /// Show or hide the exporter header
    /// </summary>
    public bool HideHeader
    {
        get => _hideHeader;
        set
        {
            _hideHeader = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(HideHeader)));
        }
    }

    private ReplayFile _replay;
    /// <summary>
    /// The main replay data to export from
    /// </summary>
    public ReplayFile Replay
    {
        get => _replay;
        set
        {
            _replay = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(Replay)));
        }
    }

    private ReplayPreview _replayPreview;
    /// <summary>
    /// The preview replay data, useful for player markers
    /// </summary>
    public ReplayPreview ReplayPreview
    {
        get => _replayPreview;
        set
        {
            _replayPreview = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(ReplayPreview)));
        }
    }

    private string _presetName;
    /// <summary>
    /// The preset name of the current configuration
    /// </summary>
    public string PresetName
    {
        get => _presetName;
        set
        {
            _presetName = value;

            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(PresetName)));
        }
    }

    private bool _manualPlayerSelection;
    /// <summary>
    /// Players will be selected manually (save names in preset)
    /// </summary>
    public bool ManualPlayerSelection
    {
        get => _manualPlayerSelection;
        set
        {
            _manualPlayerSelection = value;

            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(ManualPlayerSelection)));
        }
    }

    private bool _alwaysIncludeMarked;
    /// <summary>
    /// Check or uncheck players with marks
    /// </summary>
    public bool AlwaysIncludeMarked
    {
        get => _alwaysIncludeMarked;
        set
        {
            _alwaysIncludeMarked = value;

            // Check or uncheck marked players
            if (Players != null)
            {
                foreach (ExportPlayerSelectItem playerSelect in Players)
                {
                    // if the player is known, check it, or uncheck it based on selection
                    // do not uncheck if include all is chosen
                    if (playerSelect.PlayerPreview.IsKnownPlayer && !IncludeAllPlayers)
                    {
                        playerSelect.Checked = _alwaysIncludeMarked;
                    }
                    else if (!IncludeAllPlayers) // if I don't know you and not including all characters
                    {
                        playerSelect.Checked = false;
                    }
                }
            }

            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(AlwaysIncludeMarked)));
        }
    }

    private bool _includeAllPlayers;
    /// <summary>
    /// Always select all players
    /// </summary>
    public bool IncludeAllPlayers
    {
        get => _includeAllPlayers;
        set
        {
            _includeAllPlayers = value;

            // Check or uncheck players
            if (Players != null)
            {
                foreach (ExportPlayerSelectItem playerSelect in Players)
                {
                    playerSelect.Checked = _includeAllPlayers;
                }
            }

            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(IncludeAllPlayers)));
        }
    }

    private bool _exportAsCSV;
    /// <summary>
    /// Export data format toggle between JSON and CSV
    /// </summary>
    public bool ExportAsCSV
    {
        get => _exportAsCSV;
        set
        {
            _exportAsCSV = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(ExportAsCSV)));
        }
    }

    private bool _normalizeAttributeNames;
    /// <summary>
    /// Try to convert attribute names to camel case
    /// </summary>
    public bool NormalizeAttributeNames
    {
        get => _normalizeAttributeNames;
        set
        {
            _normalizeAttributeNames = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(NormalizeAttributeNames)));
        }
    }

    private bool _applyStaticData;
    public bool ApplyStaticData
    {
        get => _applyStaticData;
        set
        {
            _applyStaticData = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(ApplyStaticData)));
        }
    }

    private bool _includeMatchID;
    /// <summary>
    /// Include match id as a root json element
    /// </summary>
    public bool IncludeMatchID
    {
        get => _includeMatchID;
        set
        {
            _includeMatchID = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(IncludeMatchID)));
        }
    }

    private bool _includeMatchDuration;
    /// <summary>
    /// Include match duration (in ms) as root json element
    /// </summary>
    public bool IncludeMatchDuration
    {
        get => _includeMatchDuration;
        set
        {
            _includeMatchDuration = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(IncludeMatchDuration)));
        }
    }

    private bool _includePatchVersion;
    /// <summary>
    /// Include patch version as root json element
    /// </summary>
    public bool IncludePatchVersion
    {
        get => _includePatchVersion;
        set
        {
            _includePatchVersion = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(IncludePatchVersion)));
        }
    }

    private List<ExportPlayerSelectItem> _players;
    /// <summary>
    /// Collection of players used to feed player ListBox
    /// </summary>
    public List<ExportPlayerSelectItem> Players
    {
        get => _players;
        set
        {
            _players = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(Players)));
        }
    }

    private string _attributeFilterText;
    /// <summary>
    /// Attribute ListBox will be filtered on this text
    /// </summary>
    public string AttributeFilterText
    {
        get => _attributeFilterText;
        set
        {
            _attributeFilterText = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(AttributeFilterText)));
        }
    }

    private List<ExportAttributeSelectItem> _attributes;
    /// <summary>
    /// Collection of attributes
    /// </summary>
    public List<ExportAttributeSelectItem> Attributes
    {
        get => _attributes;
        set
        {
            _attributes = value;
            AttributesView = new CollectionViewSource
            {
                Source = Attributes,
                IsLiveSortingRequested = true
            };

            // Sort checked attributes at the top, followed by name
            AttributesView.SortDescriptions.Add(new SortDescription("Checked", ListSortDirection.Descending));
            AttributesView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(Attributes)));
        }
    }

    private CollectionViewSource _attributesView;
    /// <summary>
    /// View to the attributes collection, provides live filtering and sorting
    /// </summary>
    public CollectionViewSource AttributesView
    {
        get => _attributesView;
        set
        {
            _attributesView = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(AttributesView)));
        }
    }

    private string _exportPreview;
    /// <summary>
    /// String containing the output data
    /// </summary>
    public string ExportPreview
    {
        get => _exportPreview;
        set
        {
            _exportPreview = value;
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(nameof(ExportPreview)));
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create preset object based on current configuration
    /// </summary>
    /// <returns></returns>
    public ExportPreset CreatePreset()
    {
        return new ExportPreset
        {
            PresetName = PresetName,
            ManualPlayerSelection = ManualPlayerSelection,
            AlwaysIncludeMarked = AlwaysIncludeMarked,
            IncludeAllPlayers = IncludeAllPlayers,
            ExportAsCSV = ExportAsCSV,
            IncludeMatchDuration = !ExportAsCSV && IncludeMatchDuration,
            IncludeMatchID = !ExportAsCSV && IncludeMatchID,
            IncludePatchVersion = !ExportAsCSV && IncludePatchVersion,
            NormalizeAttributeNames = NormalizeAttributeNames,
            ApplyStaticData = ApplyStaticData,
            SelectedAttributes = Attributes.Where(x => x.Checked)
                                           .Select(x => x.Name)
                                           .ToList(),
            // only include players if manual selection is true
            SelectedPlayers = ManualPlayerSelection
                ? Players.Where(x => x.Checked)
                         .Select(x => x.PlayerPreview.PlayerName)
                         .ToList()
                : new List<string>()
        };
    }

    /// <summary>
    /// Load configuration saved in a preset
    /// </summary>
    /// <param name="preset"></param>
    public void LoadPreset(ExportPreset preset)
    {
        PresetName = preset.PresetName;
        ManualPlayerSelection = preset.ManualPlayerSelection;
        IncludeAllPlayers = preset.IncludeAllPlayers;
        AlwaysIncludeMarked = preset.AlwaysIncludeMarked;
        ExportAsCSV = preset.ExportAsCSV;
        IncludeMatchDuration = preset.IncludeMatchDuration;
        IncludeMatchID = preset.IncludeMatchID;
        IncludePatchVersion = preset.IncludePatchVersion;
        NormalizeAttributeNames = preset.NormalizeAttributeNames;
        ApplyStaticData = preset.ApplyStaticData;
        // check all attributes in presets
        foreach (string selectedAttributeName in preset.SelectedAttributes)
        {
            ExportAttributeSelectItem attribute = Attributes.FirstOrDefault(x => x.Name == selectedAttributeName);
            if (attribute != null)
            {
                attribute.Checked = true;
            }
        }

        // only select players like this if we aren't selecting by marks/all
        if (preset.ManualPlayerSelection)
        {
            foreach (string selectedPlayerName in preset.SelectedPlayers)
            {
                // find the player with the same name
                ExportPlayerSelectItem player = Players.FirstOrDefault(x => x.PlayerPreview.PlayerName == selectedPlayerName);
                if (player != null)
                {
                    player.Checked = true;
                }
            }
        }
    }

    #endregion
}
