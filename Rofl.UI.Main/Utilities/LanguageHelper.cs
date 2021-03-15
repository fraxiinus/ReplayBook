using Rofl.Settings.Models;
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
            }

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }
    }
}
