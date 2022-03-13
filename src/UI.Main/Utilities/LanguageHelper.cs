using Fraxiinus.ReplayBook.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities
{
    public static class LanguageHelper
    {
        /// <summary>
        /// The current language loaded into ReplayBook. Set using <see cref="SetProgramLanguage(Language)"/>
        /// </summary>
        public static Language CurrentLanguage { get; private set; }

        /// <summary>
        /// Updates resource dictionary to target language (English loaded as secondary always).
        /// Loads static data in target language.
        /// </summary>
        /// <param name="target"></param>
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
                case Language.ZhHant:
                    dict.Source = new Uri("..\\Resources\\Strings\\zh-Hant.xaml", UriKind.Relative);
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
        }

        /// <summary>
        /// Returns language code used by Riot, used mainly to load static data
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string GetRiotRegionCode(Language language)
        {
            return language switch
            {
                Language.En => "en_US",
                Language.ZhHans => "zh_CN",
                Language.ZhHant => "zh_TW",
                Language.De => "de_DE",
                Language.Es => "es_ES",
                Language.Fr => "fr_FR",
                Language.Pt => "pt_BR",
                _ => "en_US",
            };
        }
    }
}
