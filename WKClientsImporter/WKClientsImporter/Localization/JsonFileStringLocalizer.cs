using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WKClientsImporter.Localization
{
    public class JsonFileStringLocalizer : IStringLocalizer
    {
        private readonly object _sync = new object();
        private Dictionary<string, string> _strings;
        private string _currentLanguage;

        public event EventHandler LanguageChanged;

        public JsonFileStringLocalizer()
        {
            _currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName ?? "en";
            _strings = LoadStringsForLanguage(_currentLanguage);
        }

        public string CurrentLanguage
        {
            get
            {
                lock (_sync) { return _currentLanguage; }
            }
        }

        public void SetLanguage(string twoLetterISOLanguageName)
        {
            if (string.IsNullOrWhiteSpace(twoLetterISOLanguageName)) return;

            lock (_sync)
            {
                if (string.Equals(_currentLanguage, twoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase)) return;

                _currentLanguage = twoLetterISOLanguageName;
                _strings = LoadStringsForLanguage(_currentLanguage);
            }

            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<KeyValuePair<string, string>> GetAvailableLanguages()
        {
            var result = new List<KeyValuePair<string, string>>();

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDir, "Localization"),
                Path.Combine(baseDir, "Resources", "Localization")
            };

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var dir in candidates)
            {
                if (!Directory.Exists(dir)) continue;

                foreach (var file in Directory.GetFiles(dir, "*.json"))
                {
                    var code = Path.GetFileNameWithoutExtension(file);
                    if (string.IsNullOrWhiteSpace(code) || seen.Contains(code)) continue;

                    string display;
                    try
                    {
                        display = new CultureInfo(code).NativeName;
                    }
                    catch
                    {
                        display = code;
                    }

                    result.Add(new KeyValuePair<string, string>(code, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(display)));
                    seen.Add(code);
                }
            }

            if (result.Count == 0)
            {
                result.Add(new KeyValuePair<string, string>("en", "English"));
                result.Add(new KeyValuePair<string, string>("es", "Español"));
            }

            return result;
        }

        private Dictionary<string, string> LoadStringsForLanguage(string lang)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var relPaths = new[]
            {
                Path.Combine(baseDir, "Localization", $"{lang}.json"),
                Path.Combine(baseDir, "Resources", "Localization", $"{lang}.json"),
                Path.Combine(baseDir, "Localization", "en.json"),
                Path.Combine(baseDir, "Resources", "Localization", "en.json")
            };

            string path = relPaths.FirstOrDefault(File.Exists);
            if (path == null) return dict;

            try
            {
                var json = File.ReadAllText(path);
                var jobj = JObject.Parse(json);
                foreach (var prop in jobj.Properties())
                {
                    dict[prop.Name] = prop.Value.ToString();
                }
            }
            catch
            {
                // Si falla, devolver diccionario vacío para que Get devuelva la clave
            }

            return dict;
        }

        public string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return string.Empty;
            lock (_sync)
            {
                if (_strings != null && _strings.TryGetValue(key, out var val)) return val;
            }
            return key; // fallback: return key
        }

        public string Get(string key, params object[] args)
        {
            var template = Get(key);
            return args == null || args.Length == 0 ? template : string.Format(template, args);
        }
    }
}