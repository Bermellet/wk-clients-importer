using System;
using System.Collections.Generic;

namespace WKClientsImporter.Localization
{
    public interface IStringLocalizer
    {
        string Get(string key);
        string Get(string key, params object[] args);

        string CurrentLanguage { get; }

        void SetLanguage(string twoLetterISOLanguageName);

        List<KeyValuePair<string, string>> GetAvailableLanguages();

        event EventHandler LanguageChanged;
    }
}