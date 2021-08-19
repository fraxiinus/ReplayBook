using Rofl.Settings.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Rofl.UI.Main.Utilities
{
    public static class LanguageHelper
    {
        public static void SetProgramLanguage(Language target)
        {
            ResourceDictionary dict = new ResourceDictionary();
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
            }

            if (target != Language.En)
            {
                ResourceDictionary backupDict = new ResourceDictionary
                {
                    Source = new Uri("..\\Resources\\Strings\\en.xaml", UriKind.Relative)
                };
                Application.Current.Resources.MergedDictionaries.Add(backupDict);
            }

            Application.Current.Resources.MergedDictionaries.Add(dict);

            // Re-load static rune data
            RuneHelper.LoadRunes(target);
        }

        public static string[] GetFriendlyLanguageNames()
        {
            var languages = new List<string>();
            foreach (var lang in (Language[])Enum.GetValues(typeof(Language)))
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
                }
            }
            return languages.ToArray();
        }

        public static string GetAppropriateRegionForLanguage(Language language)
        {
            switch (language)
            {
                case Language.En:
                    return "en_US";
                case Language.ZhHans:
                    return "zh_CN";
                case Language.De:
                    return "de_DE";
                case Language.Es:
                    return "es_ES";
                default:
                    return "en_US";
            }
        }
    }
}
