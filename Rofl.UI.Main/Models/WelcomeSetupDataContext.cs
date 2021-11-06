using Etirps.RiZhi;
using ModernWpf.Controls;
using Rofl.Executables.Models;
using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Rofl.UI.Main.Models
{
    public class WelcomeSetupDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public RiZhi Log { get; set; }

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

        private string _pageTitle;
        /// <summary>
        /// Page title to be displayed above the page frame
        /// </summary>
        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(PageTitle)));
            }
        }

        private int _pageIndex;
        /// <summary>
        /// Page index used to display progress dots
        /// </summary>
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                _pageIndex = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(PageIndex)));
            }
        }

        private int _pageCount;
        public int PageCount
        {
            get => _pageCount;
            set
            {
                _pageCount = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(PageCount)));
            }
        }

        private bool _disableBackButton;
        public bool DisableBackButton
        {
            get => _disableBackButton;
            set
            {
                _disableBackButton = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(DisableBackButton)));
            }
        }

        private bool _disableNextButton;
        public bool DisableNextButton
        {
            get => _disableNextButton;
            set
            {
                _disableNextButton = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(DisableNextButton)));
            }
        }

        private bool _disableSkipButton;
        public bool DisableSkipButton
        {
            get => _disableSkipButton;
            set
            {
                _disableSkipButton = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(DisableSkipButton)));
            }
        }

        private bool _swapFinishButton;
        public bool SwapFinishButton
        {
            get => _swapFinishButton;
            set
            {
                _swapFinishButton = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(SwapFinishButton)));
            }
        }

        private Language _language;
        public Language Language
        {
            get => _language;
            set
            {
                _language = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Language)));
            }
        }

        private string _replayPath;
        /// <summary>
        /// The initial replay path to load replays from
        /// </summary>
        public string ReplayPath
        {
            get => _replayPath;
            set
            {
                _replayPath = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(ReplayPath)));
            }
        }

        private string _riotGamesPath;
        /// <summary>
        /// Initial directory to search for executables
        /// </summary>
        public string RiotGamesPath
        {
            get => _riotGamesPath;
            set
            {
                _riotGamesPath = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(RiotGamesPath)));
            }
        }

        private List<LeagueExecutable> _executables;
        /// <summary>
        /// Initial executables detected, to be registered
        /// </summary>
        public List<LeagueExecutable> Executables
        {
            get => _executables;
            set
            {
                _executables = value;
                PropertyChanged?.Invoke(
                    this, new PropertyChangedEventArgs(nameof(Executables)));
            }
        }
    }
}
