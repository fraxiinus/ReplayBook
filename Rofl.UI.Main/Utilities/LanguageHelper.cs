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
                    dict.Source = new System.Uri("..\\Resources\\Strings\\en.xaml", System.UriKind.Relative);
                    break;
                case Language.ZhHans:
                    dict.Source = new System.Uri("..\\Resources\\Strings\\zh-Hans.xaml", System.UriKind.Relative);
                    break;
                case Language.De:
                    dict.Source = new System.Uri("..\\Resources\\Strings\\de.xaml", System.UriKind.Relative);
                    break;
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
                }
            }
            return languages.ToArray(); ;
        }
    }
}
