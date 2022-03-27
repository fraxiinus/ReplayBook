using Fraxiinus.ReplayBook.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Fraxiinus.ReplayBook.UI.Main.Utilities
{
    public static class LanguageHelper
    {
        /// <summary>
        /// The current language loaded into ReplayBook. Set using <see cref="SetProgramLanguage(ProgramLanguage)"/>
        /// </summary>
        public static ProgramLanguage CurrentLanguage { get; private set; }

        /// <summary>
        /// Updates resource dictionary to target language (English loaded as secondary always).
        /// Loads static data in target language.
        /// </summary>
        /// <param name="target"></param>
        public static void SetProgramLanguage(ProgramLanguage target)
        {
            var dict = new ResourceDictionary();
            switch (target)
            {
                case ProgramLanguage.En:
                    dict.Source = new Uri("..\\Resources\\Strings\\en.xaml", UriKind.Relative);
                    break;
                case ProgramLanguage.ZhHans:
                    dict.Source = new Uri("..\\Resources\\Strings\\zh-Hans.xaml", UriKind.Relative);
                    break;
                case ProgramLanguage.ZhHant:
                    dict.Source = new Uri("..\\Resources\\Strings\\zh-Hant.xaml", UriKind.Relative);
                    break;
                case ProgramLanguage.De:
                    dict.Source = new Uri("..\\Resources\\Strings\\de.xaml", UriKind.Relative);
                    break;
                case ProgramLanguage.Es:
                    dict.Source = new Uri("..\\Resources\\Strings\\es.xaml", UriKind.Relative);
                    break;
                case ProgramLanguage.Fr:
                    dict.Source = new Uri("..\\Resources\\Strings\\fr.xaml", UriKind.Relative);
                    break;
                case ProgramLanguage.Pt:
                    dict.Source = new Uri("..\\Resources\\Strings\\pt.xaml", UriKind.Relative);
                    break;
                default:
                    break;
            }

            // Load english data in for backup
            if (target != ProgramLanguage.En)
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
        public static string GetRiotRegionCode(ProgramLanguage language)
        {
            return language switch
            {
                ProgramLanguage.En => "en_US",
                ProgramLanguage.ZhHans => "zh_CN",
                ProgramLanguage.ZhHant => "zh_TW",
                ProgramLanguage.De => "de_DE",
                ProgramLanguage.Es => "es_ES",
                ProgramLanguage.Fr => "fr_FR",
                ProgramLanguage.Pt => "pt_BR",
                _ => "en_US",
            };
        }
    }
}
