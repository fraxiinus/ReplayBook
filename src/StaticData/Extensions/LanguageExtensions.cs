using Fraxiinus.ReplayBook.Configuration.Models;

namespace Fraxiinus.ReplayBook.StaticData.Extensions
{
    public static class LanguageExtensions
    {
        public static string GetRiotRegionCode(this ProgramLanguage language)
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
