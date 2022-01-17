using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Rofl.UI.Main.Utilities
{
    public static class LanguageHelper
    {
        /// <summary>
        /// The current language loaded into ReplayBook. Set using <see cref="SetProgramLanguage(Language)"/>
        /// </summary>
        public static Language CurrentLanguage { get; private set; }

        public static void SetProgramLanguage(Language target)
        {
            var dict = new ResourceDictionary();
            switch (target)
            {
                case Language.En:
                    dict.Source = new Uri("..\\Resources\\Strings\\en.xaml", UriKind.Relative);
                    break;
                case Language.ZhHans:
                    dict.Source = new Uri("..\\Resources\\Strings\\zh-Hans.xaml", UriKind.Relative);
                    break;
                case Language.De:
                    dict.Source = new Uri("..\\Resources\\Strings\\de.xaml", UriKind.Relative);
                    break;
                case Language.Es:
                    dict.Source = new Uri("..\\Resources\\Strings\\es.xaml", UriKind.Relative);
                    break;
                case Language.Fr:
                    dict.Source = new Uri("..\\Resources\\Strings\\fr.xaml", UriKind.Relative);
                    break;
                case Language.Pt:
                    dict.Source = new Uri("..\\Resources\\Strings\\pt.xaml", UriKind.Relative);
                    break;
                default:
                    break;
            }

            // Load english data in for backup
            if (target != Language.En)
            {
                var backupDict = new ResourceDictionary
                {
                    Source = new Uri("..\\Resources\\Strings\\en.xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries.Add(backupDict);
            }

            // Load selected language
            Application.Current.Resources.MergedDictionaries.Add(dict);

            CurrentLanguage = target;

            // Re-load static data
            RuneHelper.LoadRunes(target);
            ItemHelper.LoadItems(target);
            ChampionHelper.LoadChampions(target);
        }

        /// <summary>
        /// Returns friendly names for all supported languages
        /// </summary>
        /// <returns></returns>
        public static string[] GetFriendlyLanguageNames()
        {
            var languages = new List<string>();
            foreach (Language lang in (Language[])Enum.GetValues(typeof(Language)))
            {
                switch (lang)
                {
                    case Language.En:
                        languages.Add("English");
                        break;
                    case Language.ZhHans:
                        languages.Add("简体中文");
                        break;
                    case Language.De:
                        languages.Add("Deutsch");
                        break;
                    case Language.Es:
                        languages.Add("Español");
                        break;
                    case Language.Fr:
                        languages.Add("Français");
                        break;
                    case Language.Pt:
                        languages.Add("Português (Brasil)");
                        break;
                    default:
                        break;
                }
            }
            return languages.ToArray();
        }

        /// <summary>
        /// Returns language code used by Riot
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string GetRiotRegionCode(Language language)
        {
            return language switch
            {
                Language.En => "en_US",
                Language.ZhHans => "zh_CN",
                Language.De => "de_DE",
                Language.Es => "es_ES",
                Language.Fr => "fr_FR",
                Language.Pt => "pt_BR",
                _ => "en_US",
            };
        }
    }
}
